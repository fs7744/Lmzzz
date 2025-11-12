using System.Collections;

namespace Lmzzz.Template.Inner;

public class ForStatement : IStatement
{
    public IStatement Value { get; set; }
    public IStatement Statement { get; set; }
    public string ItemName { get; set; }
    public string IndexName { get; set; }

    public object? Evaluate(TemplateContext context)
    {
        var v = Value.Evaluate(context);
        if (v is null) return null;

        if (v is IEnumerable e)
        {
            var index = 0;
            if (!context.Cache.TryGetValue(ItemName, out var o))
                o = null;

            if (!context.Cache.TryGetValue(IndexName, out var oi))
                oi = null;
            foreach (var item in e)
            {
                context.Cache[ItemName] = item;
                context.Cache[IndexName] = index;
                index++;

                Statement.Evaluate(context);
            }
            context.Cache[ItemName] = o;
            context.Cache[IndexName] = oi;
        }

        return null;
    }
}