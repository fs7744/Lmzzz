using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class CurrentNode : IStatement
{
    public IStatement? Child { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return Child is not null ? $"@.{Child.ToString()}" : "@";
    }
}