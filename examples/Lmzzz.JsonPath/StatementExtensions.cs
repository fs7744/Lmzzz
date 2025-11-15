using System.Text.Json;
using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath;

public static class StatementExtensions
{
    public static JsonNode? EvaluateObject<T>(this IStatement statement, T data, JsonNodeOptions? options = null)
    {
        //var root = JsonValue.Create(data, options);
        //return statement.Evaluate(new JsonPathContext()
        //{
        //    Root = root,
        //    Current = root
        //});
        return EvaluateJson(statement, JsonSerializer.Serialize(data), options);
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

    public static string ToChildString(this IStatement child, string str)
    {
        return child is null ? str : $"{str}.{child.ToString()}";
    }

    public static JsonNode? EvaluateChild(this IStatement child, JsonNode? node, JsonPathContext context)
    {
        if (child is null)
            return node;
        context.Current = node;
        return child.Evaluate(context);
    }

    public static bool IsTrue(this JsonNode? jsonNode)
    {
        return jsonNode is not null && jsonNode.GetValueKind() == System.Text.Json.JsonValueKind.True;
    }
}