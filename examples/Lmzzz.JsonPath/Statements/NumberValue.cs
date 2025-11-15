using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class NumberValue : IStatementValue
{
    public NumberValue(decimal x)
    {
        this.Value = x;
    }

    public decimal Value { get; }

    object IStatementValue.Value => Value;

    public JsonNode? Evaluate(JsonPathContext context)
    {
        return JsonValue.Create(Value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}