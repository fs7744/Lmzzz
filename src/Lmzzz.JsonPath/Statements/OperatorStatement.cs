using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class OperatorStatement : IStatement
{
    public IStatement Left { get; set; }
    public string Operator { get; set; }
    public IStatement Right { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"({Left} {Operator} {Right})";
    }
}