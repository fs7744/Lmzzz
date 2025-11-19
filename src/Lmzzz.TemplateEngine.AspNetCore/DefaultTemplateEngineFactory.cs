using Lmzzz.Chars;
using Lmzzz.Chars.Fluent;
using Lmzzz.Template;
using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Lmzzz.AspNetCoreTemplate;

public class DefaultTemplateEngineFactory : ITemplateEngineFactory
{
    public DefaultTemplateEngineFactory()
    {
        AddFieldConvertor(new RequestPathHttpContextFieldConvertor());
        AddFieldConvertor(new RequestHostHttpContextFieldConvertor());
        AddFieldConvertor(new RequestContentTypeHttpContextFieldConvertor());
        AddFieldConvertor(new RequestProtocolHttpContextFieldConvertor());
        AddFieldConvertor(new RequestQueryStringHttpContextFieldConvertor());
        AddFieldConvertor(new RequestSchemeHttpContextFieldConvertor());
        AddFieldConvertor(new RequestMethodHttpContextFieldConvertor());
        AddFieldConvertor(new RequestHasFormContentTypeHttpContextFieldConvertor());
        AddFieldConvertor(new RequestIsHttpsHttpContextFieldConvertor());
        AddFieldConvertor(new RequestContentLengthHttpContextFieldConvertor());
        AddFieldConvertor(new TraceIdentifierHttpContextFieldConvertor());
        AddFieldConvertor(new ResponseContentLengthHttpContextFieldConvertor());
        AddFieldConvertor(new ResponseContentTypeHttpContextFieldConvertor());
        AddFieldConvertor(new ResponseHasStartedHttpContextFieldConvertor());
        AddFieldConvertor(new ResponseStatusCodeHttpContextFieldConvertor());
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
        else if (statement is FunctionStatement fs)
        {
            r = OptimizeFuncStatement(fs);
        }
        else if (statement is NotStatement sn)
        {
            if (sn.Statement is ActionConditionStatement actionCondition)
            {
                var ss = actionCondition.action;
                return new ActionConditionStatement(c => !ss(c));
            }
        }
        else if (statement is AndStatement asn)
        {
            if (asn.Left is ActionConditionStatement actionCondition && asn.Right is ActionConditionStatement actionConditionr)
            {
                var ss = actionCondition.action;
                var ssr = actionConditionr.action;
                return new ActionConditionStatement(c => ss(c) && ssr(c));
            }
        }
        else if (statement is OrStatement aso)
        {
            if (aso.Left is ActionConditionStatement actionCondition && aso.Right is ActionConditionStatement actionConditionr)
            {
                var ss = actionCondition.action;
                var ssr = actionConditionr.action;
                return new ActionConditionStatement(c => ss(c) || ssr(c));
            }
        }

        return r ?? statement;
    }

    private static IStatement OptimizeFuncStatement(FunctionStatement fs)
    {
        if (funcConvertor.TryGetValue(fs.Name, out var func))
            return func(fs);
        return null;
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

    public static bool TryGetBoolFunc(IStatement statement, out Func<HttpContext, bool> func)
    {
        if (statement is FieldStatement f && fieldConvertor.TryGetValue(f.Key, out var c) && c.TryConvertBoolFunc(statement, out func))
        {
            return true;
        }

        if (statement is IHttpConditionStatement s)
        {
            func = c => s.EvaluateHttp(c) ? true : false;
            return true;
        }

        func = null;
        return false;
    }

    public static void AddFieldConvertor(HttpContextFieldConvertor c)
    {
        fieldConvertor[c.Key()] = c;
    }

    private static readonly Dictionary<string, HttpContextFieldConvertor> fieldConvertor = new Dictionary<string, HttpContextFieldConvertor>(StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, Func<FunctionStatement, IStatement>> funcConvertor = new Dictionary<string, Func<FunctionStatement, IStatement>>(StringComparer.OrdinalIgnoreCase)
    {
        { "Regex", fs=>
        {
            var ss = fs.Arguments.Length > 3 ? (fs.Arguments[3] is StringValueStatement v ? v.Value : null ): null;
            var o = Enum.TryParse<RegexOptions>(ss , true, out var r) ? r : RegexOptions.Compiled;
            if (fs.Arguments[0] is StringValueStatement s && fs.Arguments[1] is StringValueStatement svv )
            {
                var reg = new Regex(svv.Value, o);
                return new BoolConditionStatement(reg.IsMatch(s.Value));
            }
            else if (fs.Arguments[0] is FieldStatement f && fs.Arguments[1] is StringValueStatement sv
                && fieldConvertor.TryGetValue(f.Key, out var c) && c.TryConvertStringFunc(null, out var func))
            {
                var reg = new Regex(sv.Value, o);
                return new ActionConditionStatement(c => reg.IsMatch(func(c)));
            }
            else
                return null;
        } }
    };
}