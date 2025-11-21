using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Lmzzz.AspNetCoreTemplate;

public class HttpTemplateIfStatement : IfStatement, IHttpTemplateStatement
{
    public IHttpIfConditionStatement IfHttp { get; set; }
    public IEnumerable<IHttpIfConditionStatement> ElseIfsHttp { get; set; }
    public IHttpTemplateStatement? ElseHttp { get; set; }

    public void EvaluateHttpTemplate(HttpContext context, StringBuilder sb)
    {
        if (IfHttp.EvaluateHttpTemplate(context, sb))
        {
        }
        else if (ElseIfsHttp != null && ElseIfsHttp.Any(x => x.EvaluateHttpTemplate(context, sb)))
        { }
        else if (ElseHttp != null)
            ElseHttp.EvaluateHttpTemplate(context, sb);
    }
}

public class HttpTemplateForStatement : ForStatement, IHttpTemplateStatement
{
    public IObjectHttpStatement ValueHttp { get; set; }
    public IHttpTemplateStatement StatementHttp { get; set; }

    public void EvaluateHttpTemplate(HttpContext context, StringBuilder sb)
    {
        var v = ValueHttp.EvaluateObjectHttp(context);
        if (v is null) return;

        if (v is System.Collections.IEnumerable e)
        {
            var index = 0;
            if (!context.Items.TryGetValue(ItemName, out var o))
                o = null;

            if (!context.Items.TryGetValue(IndexName, out var oi))
                oi = null;
            foreach (var item in e)
            {
                context.Items[ItemName] = item;
                context.Items[IndexName] = index;
                index++;

                StatementHttp.EvaluateHttpTemplate(context, sb);
            }
            context.Items[ItemName] = o;
            context.Items[IndexName] = oi;
        }
    }
}