using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class RootNode : IStatement
{
    public IStatement? Child { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        if (Child is not null)
        {
            context.Current = context.Root;
            return Child.Evaluate(context);
        }
        return context.Root;
    }

    public override string ToString()
    {
        return Child is not null ? $"$.{Child.ToString()}" : "$";
    }
}