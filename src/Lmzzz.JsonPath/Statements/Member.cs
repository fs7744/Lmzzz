using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class Member : IParentStatement
{
    public IStatement? Child { get; set; }
    public string Name { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        if (context.Current is null)
            return null;

        if (context.Current is JsonObject o)
        {
            if (o.TryGetPropertyValue(Name, out var n))
                return n;
            else
                return null;
        }

        if (context.Current.GetValueKind() == System.Text.Json.JsonValueKind.Object)
        {
            //todo
        }

        return null;
    }

    public override string ToString()
    {
        return Child.ToChildString($"[{Name}]");
    }
}