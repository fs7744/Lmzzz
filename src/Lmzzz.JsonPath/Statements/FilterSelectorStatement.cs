using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class FilterSelectorStatement : IParentStatement
{
    public IStatement? Child { get; set; }
    public IStatement Statement { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return Child.ToChildString($"?({Statement})");
    }
}