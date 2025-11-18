using Lmzzz.Chars;
using Lmzzz.Template.Inner;
using Microsoft.Extensions.ObjectPool;
using System.Text;

namespace Lmzzz.Template;

public static class TemplateEngine
{
    private static readonly ObjectPool<StringBuilder> pool = new DefaultObjectPoolProvider().CreateStringBuilderPool();
    private static Func<IStatement, IStatement> optimizer;

    public static string EvaluateTemplate(this string template, object data, FieldStatementMode fieldMode = FieldStatementMode.Runtime)
    {
        var t = template.ToTemplate();
        return Evaluate(t, data, fieldMode);
    }

    public static IStatement ToTemplate(this string template)
    {
        if (!TemplateEngineParser.Template.TryParse(new Chars.Fluent.CharParseContext(new StringCursor(template)), out var t, out var error))
            throw error;
        return t;
    }

    public static string Evaluate(this IStatement template, object data, FieldStatementMode fieldMode = FieldStatementMode.Runtime)
    {
        var sb = pool.Get();
        try
        {
            var c = new TemplateContext(data) { StringBuilder = sb, FieldMode = fieldMode };
            template.Evaluate(c);
            return sb.ToString();
        }
        finally
        {
            pool.Return(sb);
        }
    }

    internal static T Optimize<T>(T statement) where T : IStatement
    {
        if (optimizer != null)
        {
            var o = optimizer(statement);
            if (o is T t)
            {
                return t;
            }
        }
        return statement;
    }

    public static void SetOptimizer(Func<IStatement, IStatement> optimizer)
    {
        TemplateEngine.optimizer = optimizer;
    }
}