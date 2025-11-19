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
    [InlineData("1 != null", true)]
    [InlineData("1 != true", false)]
    [InlineData("1 != '/testp'", false)]
    [InlineData("1 == null", false)]
    [InlineData("1 == true", false)]
    [InlineData("1 == '/testp'", false)]
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
    [InlineData("Request.ContentType == '/testp'", false)]
    [InlineData("Request.ContentType == 'json'", true)]
    [InlineData("Request.ContentType == null", false)]
    [InlineData("Request.ContentType == Request.ContentType", true)]
    [InlineData("Request.ContentType == F", false)]
    [InlineData("Request.ContentType == Features", false)]
    [InlineData("Request.Protocol == '/testp'", false)]
    [InlineData("Request.Protocol == 'HTTP/1.1'", true)]
    [InlineData("Request.Protocol == null", false)]
    [InlineData("Request.Protocol == Request.Protocol", true)]
    [InlineData("Request.Protocol == F", false)]
    [InlineData("Request.Protocol == Features", false)]
    [InlineData("Request.QueryString == '/testp'", false)]
    [InlineData("Request.QueryString == '?s=123&d=456&f=789'", true)]
    [InlineData("Request.QueryString == null", false)]
    [InlineData("Request.QueryString == Request.QueryString", true)]
    [InlineData("Request.QueryString == F", false)]
    [InlineData("Request.QueryString == Features", false)]
    [InlineData("Request.Scheme == '/testp'", false)]
    [InlineData("Request.Scheme == 'https'", true)]
    [InlineData("Request.Scheme == null", false)]
    [InlineData("Request.Scheme == Request.Scheme", true)]
    [InlineData("Request.Scheme == F", false)]
    [InlineData("Request.Scheme == Features", false)]
    [InlineData("Request.Method == '/testp'", false)]
    [InlineData("Request.Method == 'GET'", true)]
    [InlineData("Request.Method == null", false)]
    [InlineData("Request.Method == Request.Method", true)]
    [InlineData("Request.Method == F", false)]
    [InlineData("Request.Method == Features", false)]
    [InlineData("Request.HasFormContentType == '/testp'", false)]
    [InlineData("Request.HasFormContentType == false", true)]
    [InlineData("Request.HasFormContentType == null", false)]
    [InlineData("Request.HasFormContentType == Request.HasFormContentType", true)]
    [InlineData("Request.HasFormContentType == F", false)]
    [InlineData("Request.HasFormContentType == Features", false)]
    [InlineData("Request.IsHttps == '/testp'", false)]
    [InlineData("Request.IsHttps == true", true)]
    [InlineData("Request.IsHttps == null", false)]
    [InlineData("Request.IsHttps == Request.IsHttps", true)]
    [InlineData("Request.IsHttps == F", false)]
    [InlineData("Request.IsHttps == Features", false)]
    public void ConvertRouteFunctionTest(string text, bool r)
    {
        var f = te.ConvertRouteFunction(text);
        Assert.Equal(r, f(HttpContext));
    }
}