using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class FilterSelectorStatement : IParentStatement
{
    public IStatement? Child { get; set; }
    public IStatement Statement { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        var n = context.Current;
        if (n is null)
        {
            return null;
        }
        if (n is JsonArray array)
        {
            if (array.Count == 0)
                return Child is null ? n : null;

            return new JsonArray(array.Select(x =>
            {
                context.Current = x;
                if (IsTrue(Statement.Evaluate(context)))
                {
                    return Child.EvaluateChild(x?.DeepClone(), context);
                }
                else
                    return null;
            }).Where(static x => x is not null).ToArray());
        }
        else if (n is JsonObject o)
        {
            return new JsonArray(o.Select(x =>
            {
                context.Current = x.Value;
                if (IsTrue(Statement.Evaluate(context)))
                {
                    return Child.EvaluateChild(x.Value?.DeepClone(), context);
                }
                else
                    return null;
            }).Where(static x => x is not null).ToArray());
        }
        else if (IsTrue(Statement.Evaluate(context)))
        {
            return Child.EvaluateChild(n, context);
        }

        return null;
    }

    private bool IsTrue(JsonNode? jsonNode)
    {
        return jsonNode is not null && jsonNode.GetValueKind() == System.Text.Json.JsonValueKind.True;
    }

    public override string ToString()
    {
        return Child.ToChildString($"?({Statement})");
    }
}