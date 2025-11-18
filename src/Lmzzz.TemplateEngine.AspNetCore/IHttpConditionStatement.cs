using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public interface IHttpConditionStatement : IConditionStatement
{
    bool EvaluateHttp(HttpContext context);
}