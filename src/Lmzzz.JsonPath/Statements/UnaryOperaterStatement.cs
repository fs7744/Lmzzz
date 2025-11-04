using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class UnaryOperaterStatement : IParentStatement
{
    public IStatement? Child { get; set; }
    public string Operator { get; set; }
    public IStatement Statement { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        throw new NotImplementedException();
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