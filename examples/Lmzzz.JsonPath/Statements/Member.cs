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
            {
                //if (n is JsonArray array)
                //{
                //    if (array.Count == 0)
                //        return Child is null ? n : null;

                //    return new JsonArray(array.Select(x => Child.EvaluateChild(x?.DeepClone(), context)).Where(static x => x is not null).ToArray());
                //}

                return Child.EvaluateChild(n?.DeepClone(), context);
            }
            else
                return null;
        }

        //if (context.Current.GetValueKind() == System.Text.Json.JsonValueKind.Object)
        //{
        //    //todo
        //}

        return null;
    }

    public override string ToString()
    {
        return Child.ToChildString($"[{Name}]");
    }
}