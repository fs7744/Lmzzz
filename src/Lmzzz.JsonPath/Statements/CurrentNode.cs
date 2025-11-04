using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class CurrentNode : IParentStatement
{
    public IStatement? Child { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        return Child.EvaluateChild(context.Current, context);
    }

    public override string ToString()
    {
        return Child.ToChildString("@");
    }
}