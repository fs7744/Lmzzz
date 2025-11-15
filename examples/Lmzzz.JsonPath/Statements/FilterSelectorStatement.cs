using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class FilterSelectorStatement : IParentStatement
{
    public IStatement? Child { get; set; }
    public IStatement Statement { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        var n = context.Current;
        if (n is null)
        {
            return null;
        }
        if (n is JsonArray array)
        {
            if (array.Count == 0)
                return Child is null ? n : null;

            var list = new List<JsonNode?>();
            foreach (var x in array)
            {
                context.Current = x;
                if (Statement.Evaluate(context).IsTrue())
                {
                    if (Child is null)
                    {
                        list.Add(x?.DeepClone());
                    }
                    else
                    {
                        context.Current = x;
                        var a = Child.Evaluate(context);
                        if (a is JsonArray ja)
                        {
                            list.AddRange(ja.Select(static item => item?.DeepClone()));
                        }
                        else if (a is not null)
                            list.Add(a);
                    }
                }
            }
            return new JsonArray(list.ToArray());
            //return new JsonArray(array.Select(x =>
            //{
            //    context.Current = x;
            //    if (Statement.Evaluate(context).IsTrue())
            //    {
            //        return Child.EvaluateChild(x?.DeepClone(), context);
            //    }
            //    else
            //        return null;
            //}).Where(static x => x is not null).ToArray());
        }
        else if (n is JsonObject o)
        {
            var list = new List<JsonNode?>();
            foreach (var x in o)
            {
                context.Current = x.Value;
                if (Statement.Evaluate(context).IsTrue())
                {
                    if (Child is null)
                    {
                        list.Add(x.Value?.DeepClone());
                    }
                    else
                    {
                        context.Current = x.Value;
                        var a = Child.Evaluate(context);
                        if (a is JsonArray ja)
                        {
                            list.AddRange(ja.Select(static item => item?.DeepClone()));
                        }
                        else if (a is not null)
                            list.Add(a);
                    }
                }
            }
            return new JsonArray(list.ToArray());

            //return new JsonArray(o.Select(x =>
            //{
            //    context.Current = x.Value;
            //    if (Statement.Evaluate(context).IsTrue())
            //    {
            //        return Child.EvaluateChild(x.Value?.DeepClone(), context);
            //    }
            //    else
            //        return null;
            //}).Where(static x => x is not null).ToArray());
        }
        else if (Statement.Evaluate(context).IsTrue())
        {
            return Child.EvaluateChild(n, context);
        }

        return null;
    }

    public override string ToString()
    {
        return Child.ToChildString($"?({Statement})");
    }
}