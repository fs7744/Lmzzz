using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class RootNode : IParentStatement
{
    public IStatement? Child { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        return Child.EvaluateChild(context.Root, context);
    }

    public override string ToString()
    {
        return Child.ToChildString("$");
    }
}