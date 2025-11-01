using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class UnaryOperaterStatement : IStatement
{
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
            return $"({Statement})";
        }
        else
        {
            return $"{Operator}({Statement})";
        }
    }
}