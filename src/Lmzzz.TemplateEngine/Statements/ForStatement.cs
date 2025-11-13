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
            var c = context.Scope();
            var index = 0;
            //if (!c.Cache.TryGetValue(ItemName, out var o))
            //    o = null;

            //if (!c.Cache.TryGetValue(IndexName, out var oi))
            //    oi = null;
            foreach (var item in e)
            {
                c.Cache[ItemName] = item;
                c.Cache[IndexName] = index;
                index++;

                Statement.Evaluate(c);
            }
            //c.Cache[ItemName] = o;
            //c.Cache[IndexName] = oi;
        }

        return null;
    }
}