using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath;

public interface IStatement
{
    public JsonNode? Evaluate(JsonPathContext context);
}