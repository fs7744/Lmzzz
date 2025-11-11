using Lmzzz.Chars;
using Lmzzz.Template.Inner;
using Microsoft.Extensions.ObjectPool;
using System.Text;

namespace Lmzzz.Template;

public static class TemplateEngineExtensions
{
    private static readonly ObjectPool<StringBuilder> pool = new DefaultObjectPoolProvider().CreateStringBuilderPool();

    public static string EvaluateTemplate(this string template, object data)
    {
        var t = template.ToTemplate();
        return Evaluate(t, data);
    }

    public static IStatement ToTemplate(this string template)
    {
        if (!TemplateEngineParser.Template.TryParse(new Chars.Fluent.CharParseContext(new StringCursor(template)), out var t, out var error))
            throw error;
        return t;
    }

    public static string Evaluate(this IStatement template, object data)
    {
        var sb = pool.Get();
        try
        {
            var c = new TemplateContext(data) { StringBuilder = sb };
            template.Evaluate(c);
            return sb.ToString();
        }
        finally
        {
            pool.Return(sb);
        }
    }
}