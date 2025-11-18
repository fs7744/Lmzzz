using Lmzzz.AspNetCoreTemplate;
using Microsoft.AspNetCore.Http;

namespace UT.AspNetCoreTemplate;

public class TemplateEngineFactoryTest
{
    private readonly DefaultTemplateEngineFactory te;
    private DefaultHttpContext HttpContext;

    public TemplateEngineFactoryTest()
    {
        te = new DefaultTemplateEngineFactory();
        this.HttpContext = new DefaultHttpContext();
        var req = HttpContext.Request;
        req.Path = "/testp/dsd/fsdfx/fadasd3/中";
        req.Method = "GET";
        req.Host = new HostString("x.com");
        req.Scheme = "https";
        req.Protocol = "HTTP/1.1";
        req.ContentType = "json";
        req.QueryString = new QueryString("?s=123&d=456&f=789");
        req.IsHttps = true;
        for (int i = 0; i < 10; i++)
        {
            req.Headers.Add($"x-{i}", new string[] { $"v-{i}", $"x-{i}", $"s-{i}" });
        }
    }

    [Theory]
    [InlineData("Request.Path == '/testp'", false)]
    [InlineData("Request.Path == '/testp/dsd/fsdfx/fadasd3/中'", true)]
    [InlineData("Request.Path == null", false)]
    [InlineData("Request.Path == Request.Path", true)]
    [InlineData("Request.Path == F", false)]
    [InlineData("Request.Path == Features", false)]
    [InlineData("Request.Host == '/testp'", false)]
    [InlineData("Request.Host == 'x.com'", true)]
    [InlineData("Request.Host == null", false)]
    [InlineData("Request.Host == Request.Host", true)]
    [InlineData("Request.Host == F", false)]
    [InlineData("Request.Host == Features", false)]
    public void ConvertRouteFunctionTest(string text, bool r)
    {
        var f = te.ConvertRouteFunction(text);
        Assert.Equal(r, f(HttpContext));
    }
}