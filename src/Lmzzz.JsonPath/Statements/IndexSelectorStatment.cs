using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class IndexSelectorStatment : IParentStatement
{
    public IStatement? Child { get; set; }
    public int Index { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        if (context.Current is null)
            return null;

        if (context.Current is JsonArray array && array.Count > 0 && Index < array.Count)
        {
            if (Index < 0)
            {
                var index = array.Count + Index;
                if (index < 0)
                    return null;
                return array[index];
            }
            return array[Index];
        }

        return null;
    }

    public override string ToString()
    {
        return Child.ToChildString($"[{Index}]");
    }
}