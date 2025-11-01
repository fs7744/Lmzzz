using System.Text.Json;
using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath;

public static class StatementExtensions
{
    public static JsonNode? EvaluateObject<T>(this IStatement statement, T data, JsonNodeOptions? options = null)
    {
        var root = JsonValue.Create(data, options);
        return statement.Evaluate(new JsonPathContext()
        {
            Root = root,
            Current = root
        });
    }

    public static JsonNode? EvaluateJson(this IStatement statement, string json, JsonNodeOptions? options = null, JsonDocumentOptions documentOptions = default)
    {
        var root = json == null ? null : JsonNode.Parse(json, options, documentOptions);
        return statement.Evaluate(new JsonPathContext()
        {
            Root = root,
            Current = root
        });
    }
}