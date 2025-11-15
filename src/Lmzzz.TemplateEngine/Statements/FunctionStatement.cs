using System.Text.RegularExpressions;

namespace Lmzzz.Template.Inner;

public class FunctionStatement : IConditionStatement
{
    public string Name { get; set; }
    public IStatement[] Arguments { get; set; }

    public static readonly System.Collections.Concurrent.ConcurrentDictionary<string, Regex> Regexs = new System.Collections.Concurrent.ConcurrentDictionary<string, Regex>();

    public static readonly Dictionary<string, Func<TemplateContext, IStatement[], object?>> Functions = new Dictionary<string, Func<TemplateContext, IStatement[], object?>>(StringComparer.OrdinalIgnoreCase)
    {
        { "Regex", (context, args) =>
            {
                if(args == null || args.Length < 2) return false;
                var t = args[0].Evaluate(context);
                if(t is null || t is not string ts) return false;
                var s = args[1].Evaluate(context);
                if(s == null || s is not string rs) return false;
                var ss = args.Length >= 3 ? args[2].Evaluate(context) : null;
                var o = ss is string rss
                    ? (Enum.TryParse<RegexOptions>(rss , true, out var r) ? r : RegexOptions.Compiled)
                    : (ss is decimal d ? (RegexOptions)Convert.ToInt32(d): RegexOptions.Compiled );
                var reg = Regexs.GetOrAdd($"{rs}_{o}", static (k,oo)=> new Regex(oo.rs, oo.o), (rs, o));
                return reg.IsMatch(ts);
            }
        }
    };

    public object? Evaluate(TemplateContext context)
    {
        if (Functions.TryGetValue(Name, out var f))
            return f(context, Arguments);
        else if (context.Functions != null && context.Functions.TryGetValue(Name, out f))
            return f(context, Arguments);
        else return null;
    }

    public bool EvaluateCondition(TemplateContext context)
    {
        var r = Evaluate(context);
        if (r is bool b) return b;
        else return false;
    }
}