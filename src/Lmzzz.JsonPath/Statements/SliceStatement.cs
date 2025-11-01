using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class SliceStatement : IStatement
{
    public int? Start { get; set; }
    public int? End { get; set; }
    public int? Step { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"{(Start.HasValue ? Start.Value.ToString() : "")}:{(End.HasValue ? End.Value.ToString() : "")}:{(Step.HasValue ? Step.Value.ToString() : "")}";
    }
}