using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public interface IObjectHttpStatement
{
    object EvaluateObjectHttp(HttpContext context);
}