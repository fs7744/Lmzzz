using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class UnaryOperaterStatement : IParentStatement
{
    public IStatement? Child { get; set; }
    public string Operator { get; set; }
    public IStatement Statement { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        if (Operator.Equals("!"))
        {
            return Not(Child.EvaluateChild(Statement.Evaluate(context), context));
        }
        else
        {
            return Child.EvaluateChild(Statement.Evaluate(context), context);
        }
    }

    private JsonNode? Not(JsonNode? jsonNode)
    {
        if (jsonNode is null)
        {
            return null;
        }

        if (jsonNode.GetValueKind() == System.Text.Json.JsonValueKind.True)
        {
            return JsonValue.Create(false);
        }
        else if (jsonNode.GetValueKind() == System.Text.Json.JsonValueKind.False)
        {
            return JsonValue.Create(true);
        }
        else
        {
            return null;
        }
    }

    public override string ToString()
    {
        if (Operator.Equals("("))
        {
            return Child.ToChildString($"({Statement})");
        }
        else
        {
            return Child.ToChildString($"{Operator}({Statement})");
        }
    }
}