using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class SliceStatement : IParentStatement
{
    public IStatement? Child { get; set; }
    public int? Start { get; set; }
    public int? End { get; set; }
    public int? Step { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        if (context.Current is null || context.Current is not JsonArray a)
            return null;
        if (a.Count == 0)
            return new JsonArray();

        return new JsonArray(Slice(a, context).ToArray());
    }

    private IEnumerable<JsonNode?> Slice(JsonArray a, JsonPathContext context)
    {
        var step = Step ?? 1;
        if (step <= 0)
            yield break;

        var start = Start ?? 0;
        if (start < 0)
            start = a.Count + start;

        if (start < 0)
            start = 0;

        var end = End ?? a.Count - 1;

        if (end < 0)
            end = a.Count + end;

        if (end < 0)
            yield break;

        for (var i = start; i <= end && i < a.Count; i += step)
        {
            yield return Child.EvaluateChild(a[i]?.DeepClone(), context);
        }
    }

    public override string ToString()
    {
        return Child.ToChildString($"{(Start.HasValue ? Start.Value.ToString() : "")}:{(End.HasValue ? End.Value.ToString() : "")}:{(Step.HasValue ? Step.Value.ToString() : "")}");
    }
}