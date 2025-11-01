using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class OrStatement : IStatement
{
    public IStatement Left { get; set; }
    public IStatement Right { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"({Left.ToString()} || {Right.ToString()})";
    }
}