using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class FunctionStatement : IParentStatement
{
    public static readonly Dictionary<string, Func<JsonNode?[], JsonNode?>> Functions = new(StringComparer.OrdinalIgnoreCase)
    {
        { "length", args =>
            {
                if (args is null || args.Length == 0 || args[0] is null)
                    return null;
                if (args[0] is JsonArray arr)
                    return JsonValue.Create(new decimal(arr.Count));
                if (args[0] is JsonObject obj)
                    return JsonValue.Create(new decimal(obj.Count));
                if (args[0] is JsonValue val && val.TryGetValue<string>(out var s))
                    return JsonValue.Create(new decimal(s == null ? 0 : s.Length));
                return null;
            }
        }
    };

    public IStatement? Child { get; set; }
    public string Name { get; set; }
    public IStatement[] Arguments { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        if (!Functions.TryGetValue(Name, out var func))
        {
            if (context.Functions == null || !context.Functions.TryGetValue(Name, out func))
            {
                throw new NotSupportedException($"Not found function '{Name}'.");
            }
        }

        var args = new JsonNode?[Arguments.Length];
        var n = context.Current;
        for (int i = 0; i < Arguments.Length; i++)
        {
            context.Current = n;
            args[i] = Arguments[i].Evaluate(context);
        }
        return func(args);
    }

    public override string ToString()
    {
        return Child.ToChildString($"{Name}({string.Join(",", Arguments.Select(static x => x.ToString()))})");
    }
}