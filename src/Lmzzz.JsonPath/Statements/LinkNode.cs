using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class LinkNode : IStatement
{
    public IStatement Current { get; set; }
    public IStatement? Child { get; set; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        context.Current = Current.Evaluate(context);
        return Child?.Evaluate(context);
    }

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        IStatement cc = this;
        while (cc is not null)
        {
            if (cc is LinkNode c)
            {
                sb.Append(c.Current.ToString());
                cc = c.Child;
                if (cc is not null)
                    sb.Append(".");
            }
            else
            {
                sb.Append(cc.ToString());
                cc = null;
            }
        }
        return sb.ToString();
    }
}