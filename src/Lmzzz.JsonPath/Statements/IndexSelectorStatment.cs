using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class IndexSelectorStatment : IStatement
{
    public int Index { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        if (context.Current is null)
            return null;

        if (context.Current is JsonArray array && Index < array.Count)
            return array[Index];

        return null;
    }

    public override string ToString()
    {
        return $"[{Index}]";
    }
}