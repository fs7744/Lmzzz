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
        req.ContentLength = 1;
        HttpContext.TraceIdentifier = "t111";
        HttpContext.Response.ContentLength = 1;
        HttpContext.Response.ContentType = "json";
        HttpContext.Response.StatusCode = 400;
        for (int i = 0; i < 10; i++)
        {
            req.Headers.Add($"x-{i}", new string[] { $"v-{i}", $"x-{i}", $"s-{i}" });
            HttpContext.Response.Headers.Add($"x-{i}", new string[] { $"v-{i}", $"x-{i}", $"s-{i}" });
        }

        req.Headers.Cookie = "a=sss;b=444;";
        req.Form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues> { { "aa", "ddd" }, { "cc", "cccs" } });
    }

    [Theory]
    [InlineData("1 != null", true)]
    [InlineData("1 != true", true)]
    [InlineData("1 != '/testp'", true)]
    [InlineData("1 == null", false)]
    [InlineData("1 == true", false)]
    [InlineData("1 == '/testp'", false)]
    [InlineData("Request.Path != '/testp/dsd/fsdfx/fadasd3/中1'", true)]
    [InlineData("(Request.Path != '/testp/dsd/fsdfx/fadasd3/中1' && 1== 1 && 3 != 3) || Request.HttpContext == Request.HttpContext", true)]
    [InlineData("(Request.Path != '/testp/dsd/fsdfx/fadasd3/中1' && 1== 1 && 3 != 5.6) || Request.HttpContext == Request.HttpContext", true)]
    [InlineData("Request.HttpContext.Request.Path.Value != '/testp/dsd/fsdfx/fadasd3/中1'", true)]
    [InlineData("Request.HttpContext.Request.Path.Value == '/testp/dsd/fsdfx/fadasd3/中'", true)]
    [InlineData("Request.HttpContext == Request.HttpContext", true)]
    [InlineData("Request.HttpContext != Request.HttpContext", false)]
    [InlineData("in(Request.HttpContext.Request.Path.Value, 'a','/testp/dsd/fsdfx/fadasd3/中')", true)]
    [InlineData("Regex(Request.HttpContext.Request.Path.Value, '^[/]TEST.*', 9)", true)]
    [InlineData("inIgnoreCase(Request.HttpContext.Request.Path.Value, 'a','/testp/DSD/fsdfx/fadasd3/中')", true)]
    [InlineData("EqualIgnoreCase(Request.HttpContext.Request.Path.Value, '/testp/DSD/fsdfx/fadasd3/中')", true)]
    [InlineData("in(Request.Path, 'a','/testp/dsd/fsdfx/fadasd3/中')", true)]
    [InlineData("Regex(Request.Path, '^[/]TEST.*', 9)", true)]
    [InlineData("inIgnoreCase(Request.Path, 'a','/testp/DSD/fsdfx/fadasd3/中')", true)]
    [InlineData("EqualIgnoreCase(Request.Path, '/testp/DSD/fsdfx/fadasd3/中')", true)]
    [InlineData("TraceIdentifier == '/testp'", false)]
    [InlineData("TraceIdentifier == 't111'", true)]
    [InlineData("TraceIdentifier == null", false)]
    [InlineData("TraceIdentifier == TraceIdentifier", true)]
    [InlineData("TraceIdentifier == F", false)]
    [InlineData("TraceIdentifier == Features", false)]
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
    [InlineData("Request.HasFormContentType == true", true)]
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
    [InlineData("Request.ContentLength == '/testp'", false)]
    [InlineData("Request.ContentLength == 1", true)]
    [InlineData("Request.ContentLength == null", false)]
    [InlineData("Request.ContentLength == Request.ContentLength", true)]
    [InlineData("Request.ContentLength == F", false)]
    [InlineData("Request.ContentLength == Features", false)]
    [InlineData("Response.ContentLength == '/testp'", false)]
    [InlineData("Response.ContentLength == 1", true)]
    [InlineData("Response.ContentLength == null", false)]
    [InlineData("Response.ContentLength == Request.ContentLength", true)]
    [InlineData("Response.ContentLength == F", false)]
    [InlineData("Response.ContentLength == Features", false)]
    [InlineData("Response.ContentType == '/testp'", false)]
    [InlineData("Response.ContentType == 'json'", true)]
    [InlineData("Response.ContentType == null", false)]
    [InlineData("Response.ContentType == Response.ContentType", true)]
    [InlineData("Response.ContentType == F", false)]
    [InlineData("Response.ContentType == Features", false)]
    [InlineData("Response.HasStarted == '/testp'", false)]
    [InlineData("Response.HasStarted == false", true)]
    [InlineData("Response.HasStarted == null", false)]
    [InlineData("Response.HasStarted == Response.HasStarted", true)]
    [InlineData("Response.HasStarted == F", false)]
    [InlineData("Response.HasStarted == Features", false)]
    [InlineData("Response.StatusCode == '/testp'", false)]
    [InlineData("Response.StatusCode == 400", true)]
    [InlineData("Response.StatusCode == null", false)]
    [InlineData("Response.StatusCode == Response.StatusCode", true)]
    [InlineData("Response.StatusCode == F", false)]
    [InlineData("Response.StatusCode == Features", false)]
    [InlineData("Request.Headers.['x-3'] == '/testp'", false)]
    [InlineData("Request.Headers.['x-3'] == 'v-3,x-3,s-3'", true)]
    [InlineData("Request.Headers.['x-3'] == null", false)]
    [InlineData("Request.Headers.['x-3'] == Request.Headers.['x-3']", true)]
    [InlineData("Request.Headers.['x-3'] == F", false)]
    [InlineData("Request.Headers.['x-3'] == Features", false)]
    [InlineData("Request.Cookies.['b'] == '/testp'", false)]
    [InlineData("Request.Cookies.['b'] == '444'", true)]
    [InlineData("Request.Cookies.['b'] == null", false)]
    [InlineData("Request.Cookies.['b'] == Request.Cookies.['b']", true)]
    [InlineData("Request.Cookies.['b'] == F", false)]
    [InlineData("Request.Cookies.['b'] == Features", false)]
    [InlineData("Request.Form.['cc'] == '/testp'", false)]
    [InlineData("Request.Form.['cc'] == 'cccs'", true)]
    [InlineData("Request.Form.['cc'] == null", false)]
    [InlineData("Request.Form.['cc'] == Request.Form.['cc']", true)]
    [InlineData("Request.Form.['cc'] == F", false)]
    [InlineData("Request.Form.['cc'] == Features", false)]
    [InlineData("Request.Query.['d'] == '/testp'", false)]
    [InlineData("Request.Query.['d'] == '456'", true)]
    [InlineData("Request.Query.['d'] == null", false)]
    [InlineData("Request.Query.['d'] == Request.Query.['d']", true)]
    [InlineData("Request.Query.['d'] == F", false)]
    [InlineData("Request.Query.['d'] == Features", false)]
    [InlineData("Response.Headers.['x-3'] == '/testp'", false)]
    [InlineData("Response.Headers.['x-3'] == 'v-3,x-3,s-3'", true)]
    [InlineData("Response.Headers.['x-3'] == null", false)]
    [InlineData("Response.Headers.['x-3'] == Request.Headers.['x-3']", true)]
    [InlineData("Response.Headers.['x-3'] == F", false)]
    [InlineData("Response.Headers.['x-3'] == Features", false)]
    public void ConvertRouteFunctionTest(string text, bool r)
    {
        var f = te.ConvertRouteFunction(text);
        Assert.Equal(r, f(HttpContext));
    }
}