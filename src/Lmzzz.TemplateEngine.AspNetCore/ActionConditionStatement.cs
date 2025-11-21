using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public class ActionConditionStatement : IHttpConditionStatement
{
    internal readonly Func<HttpContext, bool> action;

    public ActionConditionStatement(Func<HttpContext, bool> action)
    {
        this.action = action;
    }

    public object? Evaluate(TemplateContext context)
    {
        return EvaluateCondition(context);
    }

    public bool EvaluateCondition(TemplateContext context)
    {
        return EvaluateHttp((HttpContext)context.Data);
    }

    public bool EvaluateHttp(HttpContext context)
    {
        return action(context);
    }

    public object EvaluateObjectHttp(HttpContext context)
    {
        return action(context);
    }
}