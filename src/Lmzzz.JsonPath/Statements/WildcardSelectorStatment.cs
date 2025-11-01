using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class WildcardSelectorStatment : IStatement
{
    public static readonly WildcardSelectorStatment Value = new();

    public JsonNode? Evaluate(JsonPathContext context)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return "*";
    }
}