using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class RootNode : IParentStatement
{
    public IStatement? Child { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        return Child.EvaluateChild(context.Root, context);
        //if (Child is null)
        //    return context.Current;
        //return Child.EvaluateChilds(context);
    }

    public override string ToString()
    {
        return Child.ToChildString("$");
    }
}