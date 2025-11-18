using Lmzzz.Chars;
using Lmzzz.Chars.Fluent;
using Lmzzz.Template;
using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public class DefaultTemplateEngineFactory : ITemplateEngineFactory
{
    public DefaultTemplateEngineFactory()
    {
        AddFieldConvertor(new RequestPathHttpContextFieldConvertor());
        AddFieldConvertor(new RequestHostHttpContextFieldConvertor());
        TemplateEngine.SetOptimizer(OptimizeTemplateEngine);
    }

    public Func<HttpContext, bool> ConvertRouteFunction(string statement)
    {
        var localResult = new ParseResult<IConditionStatement>();
        var context = new CharParseContext(new StringCursor(statement));
        var success = TemplateEngineParser.ConditionParser.Parse(context, ref localResult);

        if (success)
        {
            var s = localResult.Value;
            return s is IHttpConditionStatement v ? v.EvaluateHttp : c => s.EvaluateCondition(new TemplateContext(c));
        }
        else
            throw new ParseException("Failed", context.Cursor.Position);
    }

    public Func<HttpContext, string> ConvertTemplate(string template)
    {
        var s = TemplateEngine.ToTemplate(template);
        return c => s.Evaluate(c);
    }

    private static IStatement OptimizeTemplateEngine(IStatement statement)
    {
        var r = statement;
        if (statement is EqualStatement equalStatement)
        {
            r = OptimizeEqualStatement(equalStatement);
        }

        return r ?? statement;
    }

    private static IHttpConditionStatement OptimizeEqualStatement(EqualStatement equalStatement)
    {
        if (equalStatement.Left is FieldStatement f && fieldConvertor.TryGetValue(f.Key, out var c))
        {
            return c.ConvertEqual(equalStatement.Right);
        }
        else if (equalStatement.Right is FieldStatement fr && fieldConvertor.TryGetValue(fr.Key, out c))
        {
            return c.ConvertEqual(equalStatement.Left);
        }

        return null;
    }

    public static bool TryGetStringFunc(IStatement statement, out Func<HttpContext, string> func)
    {
        if (statement is FieldStatement f && fieldConvertor.TryGetValue(f.Key, out var c) && c.TryConvertStringFunc(statement, out func))
        {
            return true;
        }

        if (statement is IHttpConditionStatement s)
        {
            func = c => s.EvaluateHttp(c) ? true.ToString() : false.ToString();
            return true;
        }

        func = null;
        return false;
    }

    private static readonly Dictionary<string, HttpContextFieldConvertor> fieldConvertor = new Dictionary<string, HttpContextFieldConvertor>(StringComparer.OrdinalIgnoreCase);

    public static void AddFieldConvertor(HttpContextFieldConvertor c)
    {
        fieldConvertor[c.Key()] = c;
    }
}