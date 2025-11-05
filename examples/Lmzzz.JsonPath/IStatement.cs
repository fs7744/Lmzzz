using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath;

public interface IStatement
{
    public JsonNode? Evaluate(JsonPathContext context);
}

public interface IParentStatement : IStatement
{
    public IStatement? Child { get; set; }
}