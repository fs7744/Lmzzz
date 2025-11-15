using System.Text.Json;
using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class NullValue : IStatementValue
{
    public static readonly NullValue Value = new();
    public static readonly JsonDocument JsonNullDocument = JsonDocument.Parse("null");

    object IStatementValue.Value => null;

    public JsonNode? Evaluate(JsonPathContext context)
    {
        return JsonValue.Create(JsonNullDocument.RootElement);
    }

    public override string ToString()
    {
        return "null";
    }
}