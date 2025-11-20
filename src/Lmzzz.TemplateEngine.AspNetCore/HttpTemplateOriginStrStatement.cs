using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Lmzzz.AspNetCoreTemplate;

public class HttpTemplateOriginStrStatement : OriginStrStatement, IHttpTemplateStatement
{
    public HttpTemplateOriginStrStatement(string text) : base(text)
    {
    }

    public void EvaluateHttpTemplate(HttpContext context, StringBuilder sb)
    {
        sb.Append(Text);
    }
}
