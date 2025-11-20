using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Lmzzz.AspNetCoreTemplate;

public interface IHttpTemplateStatement
{
    void EvaluateHttpTemplate(HttpContext context, StringBuilder sb);
}

public class HttpTemplateFuncFieldStatement : FieldStatement, IHttpTemplateStatement
{
    private readonly Func<HttpContext, string> strFunc;

    public HttpTemplateFuncFieldStatement(IReadOnlyList<string> names, Func<HttpContext, object> func, Func<HttpContext, string> strFunc = null) : base(names)
    {
        base.func = c => func(c as HttpContext);
        base.runtimeFunc = base.func;
        this.strFunc = strFunc != null ? strFunc : c => func(c)?.ToString();
    }

    public void EvaluateHttpTemplate(HttpContext context, StringBuilder sb)
    {
        var o = strFunc(context);
        if (o != null)
            sb.Append(o);
    }
}