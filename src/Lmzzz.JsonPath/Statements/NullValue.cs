using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class NullValue : IStatementValue
{
    public static readonly NullValue Value = new();

    object IStatementValue.Value => null;

    public JsonNode? Evaluate(JsonPathContext context)
    {
        return null;
    }

    public override string ToString()
    {
        return "null";
    }
}