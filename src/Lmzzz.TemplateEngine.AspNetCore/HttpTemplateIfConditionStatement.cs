using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Lmzzz.AspNetCoreTemplate;

public class HttpTemplateIfConditionStatement : IfConditionStatement, IHttpIfConditionStatement
{
    private readonly IHttpConditionStatement httpCondition;
    private readonly IHttpTemplateStatement httpText;

    public HttpTemplateIfConditionStatement(IHttpConditionStatement condition, IHttpTemplateStatement text) : base(condition, text)
    {
        this.httpCondition = condition;
        this.httpText = text;
    }

    public bool EvaluateHttpTemplate(HttpContext context, StringBuilder sb)
    {
        if (httpCondition.EvaluateHttp(context))
        {
            httpText.EvaluateHttpTemplate(context, sb);
            return true;
        }
        return false;
    }
}