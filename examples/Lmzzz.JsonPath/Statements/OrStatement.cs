using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class OrStatement : IStatement
{
    public IStatement Left { get; set; }
    public IStatement Right { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        var n = context.Current;
        if (Left.Evaluate(context).IsTrue())
        {
            return JsonValue.Create(true);
        }

        context.Current = n;
        if (Right.Evaluate(context).IsTrue())
        {
            return JsonValue.Create(true);
        }

        return null;
    }

    public override string ToString()
    {
        return $"({Left.ToString()} || {Right.ToString()})";
    }
}