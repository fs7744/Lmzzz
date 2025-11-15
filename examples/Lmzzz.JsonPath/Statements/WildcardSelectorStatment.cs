using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class WildcardSelectorStatment : IParentStatement
{
    public IStatement? Child { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        if (context.Current is null)
            return null;

        if (Child is not null)
        {
            IEnumerable<JsonNode?> nodes;
            if (context.Current is JsonObject o)
            {
                nodes = o.Select(static i => i.Value?.DeepClone()).Where(static i => i is not null)
                        .Select(i => Child.EvaluateChild(i!, context));
            }
            else if (context.Current is JsonArray a)
            {
                nodes = a.Select(static i => i?.DeepClone()).Where(static i => i is not null)
                        .Select(i => Child.EvaluateChild(i!, context));
            }
            else
                return null;

            if (nodes is not null)
            {
                var list = new List<JsonNode?>();
                foreach (var j in nodes)
                {
                    if (j is JsonArray ja)
                    {
                        list.AddRange(ja.Select(static x => x.DeepClone()));
                    }
                    else if (j is not null)
                    {
                        list.Add(j);
                    }
                }

                return new JsonArray(list.ToArray());
            }
        }
        else
        {
            IEnumerable<JsonNode?> nodes;
            if (context.Current is JsonObject o)
            {
                nodes = o.Select(static i => i.Value?.DeepClone());
            }
            else if (context.Current is JsonArray a)
            {
                nodes = a.Select(static i => i?.DeepClone());
            }
            else
                return null;

            return new JsonArray(nodes.ToArray());
        }

        //if (context.Current.GetValueKind() == System.Text.Json.JsonValueKind.Object)
        //{
        //    //todo
        //}

        return null;
    }

    public override string ToString()
    {
        return Child.ToChildString("*");
    }
}