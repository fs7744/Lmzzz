using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using Lmzzz.AspNetCoreTemplate;
using Lmzzz.Template;
using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.ObjectPool;
using System.Text;
using System.Text.RegularExpressions;
using VKProxy.HttpRoutingStatement;
using VKProxy.TemplateStatement;

//using VKProxy.TemplateStatement;

[MemoryDiagnoser, Orderer(summaryOrderPolicy: SummaryOrderPolicy.FastestToSlowest), GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
public class Path_HttpRoutingStatementParserBenchmarks
{
    private TemplateStatementFactory f = new TemplateStatementFactory(new DefaultObjectPoolProvider());
    private DefaultHttpContext HttpContext;

    private Func<HttpContext, bool> _PathEqual;
    private Func<HttpContext, bool> _PathEqualV2;
    private Func<HttpContext, bool> _PathEqualTrue;
    private Func<HttpContext, bool> _PathEqualTrueV2;
    private readonly Func<HttpContext, bool> _PathIn;
    private readonly Func<HttpContext, bool> _PathInV2;
    private readonly HashSet<string> set;
    private Func<HttpContext, bool> _LmzzzPathIn;
    private readonly Regex queryRegx;
    private readonly Func<HttpContext, bool> _PathComplex;
    private readonly Func<HttpContext, bool> _PathComplexV2;
    private readonly Func<HttpContext, bool> _IsHttps;
    private readonly Func<HttpContext, bool> _IsHttpsV2;
    private readonly Func<HttpContext, bool> _LmzzzIsHttps;
    private readonly Regex headersRegex;
    private readonly Func<HttpContext, bool> _headersRegex;
    private readonly Func<HttpContext, bool> _headersRegexV2;
    private readonly Func<HttpContext, bool> _LmzzzHeadersRegex;
    private readonly Func<HttpContext, string> _t;
    private readonly Func<HttpContext, string> _LmzzzT;
    private readonly Func<HttpContext, string> _LmzzzT2;
    private readonly Func<HttpContext, string> _LmzzzTemplateIF;
    private readonly Func<HttpContext, string> _LmzzzTemplateFor;
    private readonly Func<HttpContext, bool> _LmzzzEqual;
    private Regex regx;
    private Func<HttpContext, bool> _PathRegx;
    private Func<HttpContext, bool> _PathRegxV2;
    private Func<HttpContext, bool> _LmzzzRegx;
    private Func<HttpContext, bool> _LmzzzEqualTrue;
    private TemplateContext dcontext;
    private Func<HttpContext, bool> _LmzzzPathComplex;

    public Path_HttpRoutingStatementParserBenchmarks()
    {
        ITemplateEngineFactory te = new DefaultTemplateEngineFactory();
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

        queryRegx = new Regex(@"s[=].*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        regx = new Regex(@"^[/]testp.*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        _PathRegx = StatementParser.ConvertToFunc("Path ~= '^[/]testp.*'");
        _PathRegxV2 = HttpRoutingStatementParser.ConvertToFunction("Path ~= '^[/]testp.*'");
        _LmzzzRegx = te.ConvertRouteFunction("Regex(Request.Path,'^[/]testp.*', 9)");
        _PathEqual = StatementParser.ConvertToFunc("Path = '/testp'");
        _PathEqualV2 = HttpRoutingStatementParser.ConvertToFunction("Path = '/testp'");
        _LmzzzEqual = te.ConvertRouteFunction("Request.Path == '/testp'");
        //Lmzzz.Template.Inner.TemplateEngineParser.ConditionParser.TryParse("Request.Path == '/testp'", out _LmzzzEqual, out var error);
        _PathEqualTrue = StatementParser.ConvertToFunc("Path = '/testp/dsd/fsdfx/fadasd3/中'");
        _PathEqualTrueV2 = HttpRoutingStatementParser.ConvertToFunction("Path = '/testp/dsd/fsdfx/fadasd3/中'");
        _LmzzzEqualTrue = te.ConvertRouteFunction("Request.Path == '/testp/dsd/fsdfx/fadasd3/中'");
        //Lmzzz.Template.Inner.TemplateEngineParser.ConditionParser.TryParse("Request.Path == '/testp/dsd/fsdfx/fadasd3/中'", out _LmzzzEqualTrue, out error);

        _PathIn = StatementParser.ConvertToFunc("Path in ('/testp','/testp/DSD/fsdfx/fadasd3/中')");
        _PathInV2 = HttpRoutingStatementParser.ConvertToFunction("Path in ('/testp','/testp/DSD/fsdfx/fadasd3/中')");
        set = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "/testp", "/testp/DSD/fsdfx/fadasd3/中" };
        _LmzzzPathIn = te.ConvertRouteFunction("InIgnoreCase(Request.Path,'/testp','/testp/DSD/fsdfx/fadasd3/中')");

        var w = "IsHttps = true and Path = '/testp/dsd/fsdfx/fadasd3/中' AND Method = \"GET\" AND Host = \"x.com\" AND Scheme = \"https\" AND Protocol = \"HTTP/1.1\" AND ContentType = \"json\" AND QueryString ~= 's[=].*' and not(Scheme = \"http\")";
        _PathComplex = StatementParser.ConvertToFunc(w);
        _PathComplexV2 = HttpRoutingStatementParser.ConvertToFunction(w);
        _LmzzzPathComplex = te.ConvertRouteFunction("Request.IsHttps == true and Request.Path == '/testp/dsd/fsdfx/fadasd3/中' AND Request.Method == \"GET\" AND Request.Host == \"x.com\" AND Request.Scheme == \"https\" AND Request.Protocol == \"HTTP/1.1\" AND Request.ContentType == \"json\" AND Regex(Request.QueryString,'s[=].*') and !(Request.Scheme == \"http\")");

        w = "IsHttps = true";
        _IsHttps = StatementParser.ConvertToFunc(w);
        _IsHttpsV2 = HttpRoutingStatementParser.ConvertToFunction(w);
        _LmzzzIsHttps = te.ConvertRouteFunction("Request.IsHttps == true");

        headersRegex = new Regex(@"x[-].*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        w = "header('x-3') ~= 'x[-].*'";
        _headersRegex = StatementParser.ConvertToFunc(w);
        _headersRegexV2 = HttpRoutingStatementParser.ConvertToFunction(w);
        _LmzzzHeadersRegex = te.ConvertRouteFunction("Regex(Request.Headers.['x-3'],'x[-].*', 9)");

        _t = f.Convert("{Path}####{Scheme}#{header('x-3')}");
        _LmzzzT = te.ConvertTemplate("{{Request.Path}}####{{Request.Scheme}}#{{Request.Headers.['x-3']}}");
        _LmzzzT2 = te.ConvertTemplate("{{Request.Path}}####{{Request.Scheme}}#{{Request.Headers.['x-3'].0}}");
        _LmzzzTemplateIF = te.ConvertTemplate("{{if(Request.Path == null)}}{{Request.Scheme}}{{elseif(Request.Path == '/test')}}{{Request.Path}}{{else}}go{{endif}}");
        _LmzzzTemplateFor = te.ConvertTemplate("{{for(_v,_i in Request.Headers)}}{{_v}}:{{_i}}{{endfor}}");

        //EqualStatement.EqualityComparers[typeof(PathString)] = (l, r) =>
        //{
        //    if (l is PathString pl)
        //    {
        //        if (pl.HasValue)
        //        {
        //            if (r is PathString pr)
        //            {
        //                return pl == pr;
        //            }
        //            else if (r is string rs)
        //            {
        //                return pl.Value == rs;
        //            }
        //        }
        //        else
        //            return false;
        //    }
        //    return false;
        //};
        dcontext = new Lmzzz.Template.Inner.TemplateContext(HttpContext)
        {
            FieldMode = FieldStatementMode.Defined
        };
        //var fs = FieldStatement.getters.GetOrAdd("field_Request.Path", static k => new ConcurrentDictionary<Type, Func<object, object>>());
        //fs.GetOrAdd(typeof(DefaultHttpContext), t => (o) => ((HttpContext)o).Request.Path.Value);
    }

    [Benchmark(Baseline = true), BenchmarkCategory("PathEqual")]
    public void PathEqualString()
    {
        var b = HttpContext.Request.Path.Value == "/testp";
        var c = HttpContext.Request.Path.Value == "/testp/dsd/fsdfx/fadasd3/中";
    }

    [Benchmark, BenchmarkCategory("PathEqual")]
    public void PathEqual()
    {
        var b = _PathEqual(HttpContext);
        var c = _PathEqualTrue(HttpContext);
    }

    [Benchmark, BenchmarkCategory("PathEqual")]
    public void PathEqualV2()
    {
        var b = _PathEqualV2(HttpContext);
        var c = _PathEqualTrueV2(HttpContext);
    }

    [Benchmark, BenchmarkCategory("PathEqual")]
    public void LmzzzPathEqual()
    {
        var b = _LmzzzEqual(HttpContext);
        var c = _LmzzzEqualTrue(HttpContext);
    }

    [Benchmark(Baseline = true), BenchmarkCategory("Regx")]
    public void PathRegxString()
    {
        var b = regx.IsMatch(HttpContext.Request.Path.Value);
    }

    [Benchmark, BenchmarkCategory("Regx")]
    public void PathRegx()
    {
        var b = _PathRegx(HttpContext);
    }

    [Benchmark, BenchmarkCategory("Regx")]
    public void PathRegxV2()
    {
        var b = _PathRegxV2(HttpContext);
    }

    [Benchmark, BenchmarkCategory("Regx")]
    public void LmzzzRegx()
    {
        var b = _LmzzzRegx(HttpContext);
    }

    [Benchmark(Baseline = true), BenchmarkCategory("In")]
    public void PathInString()
    {
        var b = set.Contains(HttpContext.Request.Path.Value);
    }

    [Benchmark, BenchmarkCategory("In")]
    public void PathIn()
    {
        var b = _PathIn(HttpContext);
    }

    [Benchmark, BenchmarkCategory("In")]
    public void PathInV2()
    {
        var b = _PathInV2(HttpContext);
    }

    [Benchmark, BenchmarkCategory("In")]
    public void LmzzzPathIn()
    {
        var b = _LmzzzPathIn(HttpContext);
    }

    [Benchmark(Baseline = true), BenchmarkCategory("bool")]
    public void IsHttps()
    {
        var b = HttpContext.Request.IsHttps == true;
    }

    [Benchmark, BenchmarkCategory("bool")]
    public void IsHttpsp()
    {
        var b = _IsHttps(HttpContext);
    }

    [Benchmark, BenchmarkCategory("bool")]
    public void IsHttpspV2()
    {
        var b = _IsHttpsV2(HttpContext);
    }

    [Benchmark, BenchmarkCategory("bool")]
    public void LmzzzIsHttps()
    {
        var b = _LmzzzIsHttps(HttpContext);
    }

    [Benchmark(Baseline = true), BenchmarkCategory("Complex")]
    public void Complex()
    {
        var req = HttpContext.Request;
        var b = req.IsHttps == true
            && req.Path.Value == "/testp/dsd/fsdfx/fadasd3/中"
            && req.Method == "GET"
            && req.Host.Value == "x.com"
            && req.Scheme == "https"
            && req.Protocol == "HTTP/1.1"
            && req.ContentType == "json"
            && queryRegx.IsMatch(req.QueryString.Value)
            && !(req.Scheme == "http");
    }

    [Benchmark, BenchmarkCategory("Complex")]
    public void Complexp()
    {
        var b = _PathComplex(HttpContext);
    }

    [Benchmark, BenchmarkCategory("Complex")]
    public void ComplexpV2()
    {
        var b = _PathComplexV2(HttpContext);
    }

    [Benchmark, BenchmarkCategory("Complex")]
    public void LmzzzPathComplex()
    {
        var b = _LmzzzPathComplex(HttpContext);
    }

    [Benchmark(Baseline = true), BenchmarkCategory("HeadersRegex")]
    public void HeadersRegex()
    {
        var req = HttpContext.Request;
        var b = headersRegex.IsMatch(req.Headers["x-3"].ToString());
    }

    [Benchmark, BenchmarkCategory("HeadersRegex")]
    public void HeadersRegexp()
    {
        var b = _headersRegex(HttpContext);
    }

    [Benchmark, BenchmarkCategory("HeadersRegex")]
    public void HeadersRegexpV2()
    {
        var b = _headersRegexV2(HttpContext);
    }

    [Benchmark, BenchmarkCategory("HeadersRegex")]
    public void LmzzzHeadersRegex()
    {
        var b = _LmzzzHeadersRegex(HttpContext);
    }

    [Benchmark(Baseline = true), BenchmarkCategory("Template")]
    public void Template()
    {
        var req = HttpContext.Request;
        var b = $"{req.Path.Value}#{{}}{req.Scheme}#{req.Headers["x-3"]}";
    }

    [Benchmark, BenchmarkCategory("Template")]
    public void TemplateF()
    {
        var b = _t(HttpContext);
    }

    [Benchmark, BenchmarkCategory("Template")]
    public void LmzzzTemplate()
    {
        var b = _LmzzzT(HttpContext);
    }

    [Benchmark, BenchmarkCategory("Template")]
    public void LmzzzTemplate2()
    {
        var b = _LmzzzT2(HttpContext);
    }

    [Benchmark(Baseline = true), BenchmarkCategory("TemplateIF")]
    public void TemplateIF()
    {
        var req = HttpContext.Request;
        var b = $"{(req.Path.Value == null ? req.Scheme : (req.Path.Value == "/test" ? req.Path.Value : "go"))}";
    }

    [Benchmark, BenchmarkCategory("TemplateIF")]
    public void LmzzzTemplateIF()
    {
        var b = _LmzzzTemplateIF(HttpContext);
    }

    [Benchmark(Baseline = true), BenchmarkCategory("TemplateFor")]
    public void TemplateFor()
    {
        var sb = new StringBuilder();
        var req = HttpContext.Request;
        var i = 0;
        foreach (var item in req.Headers)
        {
            sb.Append($"{item.ToString()}:{i++}");
        }
        var b = sb.ToString();
    }

    [Benchmark, BenchmarkCategory("TemplateFor")]
    public void LmzzzTemplateFor()
    {
        var b = _LmzzzTemplateFor(HttpContext);
    }
}