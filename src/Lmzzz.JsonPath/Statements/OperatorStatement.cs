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
                return JsonNodeEquals(l, r) ? JsonValue.Create(true) : JsonValue.Create(false);

            case "!=":
                return JsonNodeEquals(l, r) ? JsonValue.Create(false) : JsonValue.Create(true);

            //case "<=":
            //    return;

            //case "<":
            //    return;

            //case ">=":
            //    return;

            //case ">":
            //    return;

            default:
                return null;
        }
    }

    public static bool JsonNodeEquals(JsonNode? l, JsonNode? r)
    {
        if (l is null)
            return r is null;
        switch (l.GetValueKind())
        {
            case System.Text.Json.JsonValueKind.String:
                return r is not null && r.GetValueKind() == System.Text.Json.JsonValueKind.String && l.GetValue<string>() == r.GetValue<string>();

            case System.Text.Json.JsonValueKind.Number:
                return r is not null && r.GetValueKind() == System.Text.Json.JsonValueKind.Number && l.GetValue<decimal>() == r.GetValue<decimal>();

            case System.Text.Json.JsonValueKind.True:
                return r is not null && r.GetValueKind() == System.Text.Json.JsonValueKind.True;

            case System.Text.Json.JsonValueKind.False:
                return r is not null && r.GetValueKind() == System.Text.Json.JsonValueKind.False;

            case System.Text.Json.JsonValueKind.Null:
                return r is null || r.GetValueKind() == System.Text.Json.JsonValueKind.Null;

            default:
                return false;
        }
    }

    public override string ToString()
    {
        return $"({Left} {Operator} {Right})";
    }
}