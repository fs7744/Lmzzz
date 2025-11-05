using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

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
        },
        { "count", args =>
            {
                if (args is null || args.Length == 0 || args[0] is null)
                    return JsonValue.Create(new decimal(0));
                //if (args[0] is JsonArray arr)
                //{
                //    return JsonValue.Create(new decimal(arr.Count));
                //}
                return JsonValue.Create(new decimal(1));
            }
        },
        { "value", args =>
            {
                if (args is null || args.Length == 0 || args[0] is null)
                    return null;
                //if (args[0] is JsonArray arr)
                //{
                //    return JsonValue.Create(new decimal(arr.Count));
                //}
                return args[0];
            }
        },
        { "search", args =>
            {
                if (args is null || args.Length != 2 || args[0] is not JsonValue value || args[1] is not JsonValue pattern)
                    return null;
                if (!value.TryGetValue<string>(out var text)) return JsonValue.Create(false);
                if (!pattern.TryGetValue<string>(out var regex)) return JsonValue.Create(false);
                return Regex.IsMatch(text, regex, RegexOptions.ECMAScript);
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