using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class OperatorStatement : IStatement
{
    public IStatement Left { get; set; }
    public string Operator { get; set; }
    public IStatement Right { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        var n = context.Current;
        var l = Left.Evaluate(context);
        context.Current = n;
        var r = Right.Evaluate(context);
        switch (Operator)
        {
            case "==":
                return JsonNode.DeepEquals(l, r) ? JsonValue.Create(true) : JsonValue.Create(false);

            case "!=":
                return JsonNode.DeepEquals(l, r) ? JsonValue.Create(false) : JsonValue.Create(true);

            case "<=":
                {
                    if (l is null || r is null)
                        return JsonValue.Create(false);
                    switch (l.GetValueKind())
                    {
                        case System.Text.Json.JsonValueKind.Number:
                            return r is not null && r.GetValueKind() == System.Text.Json.JsonValueKind.Number && l.AsValue().TryGetValue<decimal>(out var lv) && r.AsValue().TryGetValue<decimal>(out var rv) && lv <= rv ? JsonValue.Create(true) : JsonValue.Create(false);

                        default:
                            return JsonValue.Create(false);
                    }
                }

            case "<":
                {
                    if (l is null || r is null)
                        return JsonValue.Create(false);
                    switch (l.GetValueKind())
                    {
                        case System.Text.Json.JsonValueKind.Number:
                            return r is not null && r.GetValueKind() == System.Text.Json.JsonValueKind.Number && l.AsValue().TryGetValue<decimal>(out var lv) && r.AsValue().TryGetValue<decimal>(out var rv) && lv < rv ? JsonValue.Create(true) : JsonValue.Create(false);

                        default:
                            return JsonValue.Create(false);
                    }
                }

            case ">=":
                {
                    if (l is null || r is null)
                        return JsonValue.Create(false);
                    switch (l.GetValueKind())
                    {
                        case System.Text.Json.JsonValueKind.Number:
                            return r is not null && r.GetValueKind() == System.Text.Json.JsonValueKind.Number && l.AsValue().TryGetValue<decimal>(out var lv) && r.AsValue().TryGetValue<decimal>(out var rv) && lv >= rv ? JsonValue.Create(true) : JsonValue.Create(false);

                        default:
                            return JsonValue.Create(false);
                    }
                }

            case ">":
                {
                    if (l is null || r is null)
                        return JsonValue.Create(false);
                    switch (l.GetValueKind())
                    {
                        case System.Text.Json.JsonValueKind.Number:
                            return r is not null && r.GetValueKind() == System.Text.Json.JsonValueKind.Number && l.AsValue().TryGetValue<decimal>(out var lv) && r.AsValue().TryGetValue<decimal>(out var rv) && lv > rv ? JsonValue.Create(true) : JsonValue.Create(false);

                        default:
                            return JsonValue.Create(false);
                    }
                }

            default:
                return null;
        }
    }

    public override string ToString()
    {
        return $"({Left} {Operator} {Right})";
    }
}