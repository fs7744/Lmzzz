using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class IndexSelectorStatment : IStatement
{
    public int Index { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"[{Index}]";
    }
}