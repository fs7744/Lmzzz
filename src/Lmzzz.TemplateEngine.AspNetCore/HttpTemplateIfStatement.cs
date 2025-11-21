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