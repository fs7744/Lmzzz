using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public class BoolConditionStatement : IHttpConditionStatement
{
    internal readonly bool value;

    public BoolConditionStatement(bool v)
    {
        this.value = v;
    }

    public object? Evaluate(TemplateContext context)
    {
        return value;
    }

    public bool EvaluateCondition(TemplateContext context)
    {
        return value;
    }

    public bool EvaluateHttp(HttpContext context)
    {
        return value;
    }
}