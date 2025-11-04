using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class WildcardSelectorStatment : IParentStatement
{
    public IStatement? Child { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        if (context.Current is null)
            return null;

        if (context.Current is JsonObject o)
        {
            return new JsonArray(o.Select(static i => i.Value?.DeepClone()).ToArray());
        }

        if (context.Current.GetValueKind() == System.Text.Json.JsonValueKind.Object)
        {
            //todo
        }

        return null;
    }

    public override string ToString()
    {
        return Child.ToChildString("*");
    }
}