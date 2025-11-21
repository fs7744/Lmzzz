using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public interface IHttpConditionStatement : IConditionStatement, IObjectHttpStatement
{
    bool EvaluateHttp(HttpContext context);
}

public interface IObjectHttpStatement
{
    object EvaluateObjectHttp(HttpContext context);
}