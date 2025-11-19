using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public class BoolConditionStatement : BoolValueStatement, IHttpConditionStatement
{
    public BoolConditionStatement(bool v) : base(v)
    {
    }

    public bool EvaluateHttp(HttpContext context)
    {
        return Value;
    }
}