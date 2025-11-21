using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Lmzzz.AspNetCoreTemplate;

public class HttpTemplateReplaceStatementFunc : ReplaceStatement, IHttpTemplateStatement
{
    private readonly Func<HttpContext, string> func;

    public HttpTemplateReplaceStatementFunc(IStatement statement, Func<HttpContext, string> func) : base(statement)
    {
        this.func = func;
    }

    public void EvaluateHttpTemplate(HttpContext context, StringBuilder sb)
    {
        sb.Append(func(context));
    }
}