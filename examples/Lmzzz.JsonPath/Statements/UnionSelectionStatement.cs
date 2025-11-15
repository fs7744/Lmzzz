using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class UnionSelectionStatement : IParentStatement
{
    public IStatement? Child { get; set; }

    public UnionSelectionStatement(List<IStatement> list)
    {
        List = list.ToArray();
    }

    public IStatement[] List { get; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        if (List == null || List.Length == 0 || context.Current is null)
            return null;

        return new JsonArray(Find(context).ToArray());
    }

    private IEnumerable<JsonNode?> Find(JsonPathContext context)
    {
        var n = context.Current;
        foreach (var statement in List)
        {
            context.Current = n;
            var result = Child.EvaluateChild(statement.Evaluate(context), context);
            if (result is not null)
                yield return result;
        }
    }

    public override string ToString()
    {
        return Child.ToChildString($"[{string.Join(",", List.Select(static x => x.ToString()))}]");
    }
}