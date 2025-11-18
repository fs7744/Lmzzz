using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public interface ITemplateEngineFactory
{
    public Func<HttpContext, string> ConvertTemplate(string template);

    Func<HttpContext, bool> ConvertRouteFunction(string statement);
}