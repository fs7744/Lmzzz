using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using Lmzzz;
using Lmzzz.Chars.Fluent;
using Lmzzz.Template;
using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using VKProxy.HttpRoutingStatement;

//using VKProxy.TemplateStatement;

[MemoryDiagnoser, Orderer(summaryOrderPolicy: SummaryOrderPolicy.FastestToSlowest), GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
public class Path_HttpRoutingStatementParserBenchmarks
{
    //private TemplateStatementFactory f = new TemplateStatementFactory(new DefaultObjectPoolProvider());
    private DefaultHttpContext HttpContext;

    private Func<HttpContext, bool> _PathEqual;
    private Func<HttpContext, bool> _PathEqualV2;
    private Func<HttpContext, bool> _PathEqualTrue;
    private Func<HttpContext, bool> _PathEqualTrueV2;
    private readonly Func<HttpContext, bool> _PathIn;
    private readonly Func<HttpContext, bool> _PathInV2;
    private readonly HashSet<string> set;
    private readonly Regex queryRegx;
    private readonly Func<HttpContext, bool> _PathComplex;
    private readonly Func<HttpContext, bool> _PathComplexV2;
    private readonly Func<HttpContext, bool> _IsHttps;
    private readonly Func<HttpContext, bool> _IsHttpsV2;
    private readonly Regex headersRegex;
    private readonly Func<HttpContext, bool> _headersRegex;
    private readonly Func<HttpContext, bool> _headersRegexV2;
    private readonly Func<HttpContext, string> _t;
    private readonly Lmzzz.Template.Inner.IConditionStatement _LmzzzEqual;
    private Regex regx;
    private Func<HttpContext, bool> _PathRegx;
    private Func<HttpContext, bool> _PathRegxV2;
    private Lmzzz.Template.Inner.IConditionStatement _LmzzzEqualTrue;
    private TemplateContext dcontext;
    private IConditionStatement _LmzzzPathComplex;

    public Path_HttpRoutingStatementParserBenchmarks()
    {
        TemplateEngine.SetOptimizer(s =>
        {
            if (s is EqualStatement equalStatement)
            {
                if (equalStatement.Left is FieldStatement f)
                {
                    if (f.ToString().Equals("field_Request.Path", StringComparison.OrdinalIgnoreCase))
                    {
                        if (equalStatement.Right is StringValueStatement sv)
                        {
                            var ss = sv.Value;
                            return new ActionConditionStatement(c => string.Equals(((HttpContext)c.Data).Request.Path.Value, ss, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                    else if (f.ToString().Equals("field_Request.IsHttps", StringComparison.OrdinalIgnoreCase))
                    {
                        if (equalStatement.Right is BoolValueStatement sv)
                        {
                            var ss = sv.Value;
                            return new ActionConditionStatement(ss ? (c => ((HttpContext)c.Data).Request.IsHttps) : c => !((HttpContext)c.Data).Request.IsHttps);
                        }
                    }
                    else if (f.ToString().Equals("field_Request.Method", StringComparison.OrdinalIgnoreCase))
                    {
                        if (equalStatement.Right is StringValueStatement sv)
                        {
                            var ss = sv.Value;
                            return new ActionConditionStatement(c => string.Equals(((HttpContext)c.Data).Request.Method, ss, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                    else if (f.ToString().Equals("field_Request.Host", StringComparison.OrdinalIgnoreCase))
                    {
                        if (equalStatement.Right is StringValueStatement sv)
                        {
                            var ss = sv.Value;
                            return new ActionConditionStatement(c => string.Equals(((HttpContext)c.Data).Request.Host.ToString(), ss, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                    else if (f.ToString().Equals("field_Request.Scheme", StringComparison.OrdinalIgnoreCase))
                    {
                        if (equalStatement.Right is StringValueStatement sv)
                        {
                            var ss = sv.Value;
                            return new ActionConditionStatement(c => string.Equals(((HttpContext)c.Data).Request.Scheme, ss, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                    else if (f.ToString().Equals("field_Request.Protocol", StringComparison.OrdinalIgnoreCase))
                    {
                        if (equalStatement.Right is StringValueStatement sv)
                        {
                            var ss = sv.Value;
                            return new ActionConditionStatement(c => string.Equals(((HttpContext)c.Data).Request.Protocol, ss, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                    else if (f.ToString().Equals("field_Request.ContentType", StringComparison.OrdinalIgnoreCase))
                    {
                        if (equalStatement.Right is StringValueStatement sv)
                        {
                            var ss = sv.Value;
                            return new ActionConditionStatement(c => string.Equals(((HttpContext)c.Data).Request.ContentType, ss, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                }
            }
            else if (s is NotStatement sn)
            {
                if (sn.Statement is ActionConditionStatement actionCondition)
                {
                    var ss = actionCondition.action;
                    return new ActionConditionStatement(c => !ss(c));
                }
            }
            else if (s is AndStatement asn)
            {
                if (asn.Left is ActionConditionStatement actionCondition && asn.Right is ActionConditionStatement actionConditionr)
                {
                    var ss = actionCondition.action;
                    var ssr = actionConditionr.action;
                    return new ActionConditionStatement(c => ss(c) && ssr(c));
                }
            }
            else if (s is FunctionStatement fs)
            {
                if (fs.Name.Equals("Regex"))
                {
                    if (fs.Arguments[0] is FieldStatement f && f.ToString().Equals("field_Request.QueryString", StringComparison.OrdinalIgnoreCase) && fs.Arguments[1] is StringValueStatement sv)
                    {
                        var reg = new Regex(sv.Value, RegexOptions.Compiled);
                        return new ActionConditionStatement(c => reg.IsMatch(((HttpContext)c.Data).Request.QueryString.Value));
                    }
                }
            }
            return s;
        });

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
        _PathEqual = StatementParser.ConvertToFunc("Path = '/testp'");
        _PathEqualV2 = HttpRoutingStatementParser.ConvertToFunction("Path = '/testp'");
        Lmzzz.Template.Inner.TemplateEngineParser.ConditionParser.TryParse("Request.Path == '/testp'", out _LmzzzEqual, out var error);
        _PathEqualTrue = StatementParser.ConvertToFunc("Path = '/testp/DSD/fsdfx/fadasd3/中'");
        _PathEqualTrueV2 = HttpRoutingStatementParser.ConvertToFunction("Path = '/testp/DSD/fsdfx/fadasd3/中'");
        Lmzzz.Template.Inner.TemplateEngineParser.ConditionParser.TryParse("Request.Path == '/testp/dsd/fsdfx/fadasd3/中'", out _LmzzzEqualTrue, out error);

        _PathIn = StatementParser.ConvertToFunc("Path in ('/testp','/testp/DSD/fsdfx/fadasd3/中')");
        _PathInV2 = HttpRoutingStatementParser.ConvertToFunction("Path in ('/testp','/testp/DSD/fsdfx/fadasd3/中')");
        set = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "/testp", "/testp/DSD/fsdfx/fadasd3/中" };

        var w = "IsHttps = true and Path = '/testp/DSD/fsdfx/fadasd3/中' AND Method = \"GET\" AND Host = \"x.com\" AND Scheme = \"https\" AND Protocol = \"HTTP/1.1\" AND ContentType = \"json\" AND QueryString ~= 's[=].*' and not(Scheme = \"http\")";
        _PathComplex = StatementParser.ConvertToFunc(w);
        _PathComplexV2 = HttpRoutingStatementParser.ConvertToFunction(w);
        Lmzzz.Template.Inner.TemplateEngineParser.ConditionParser.TryParse("Request.IsHttps == true and Request.Path == '/testp/DSD/fsdfx/fadasd3/中' AND Request.Method == \"GET\" AND Request.Host == \"x.com\" AND Request.Scheme == \"https\" AND Request.Protocol == \"HTTP/1.1\" AND Request.ContentType == \"json\" AND Regex(Request.QueryString,'s[=].*') and !(Request.Scheme == \"http\")", out _LmzzzPathComplex, out error);

        w = "IsHttps = true";
        _IsHttps = StatementParser.ConvertToFunc(w);
        _IsHttpsV2 = HttpRoutingStatementParser.ConvertToFunction(w);

        headersRegex = new Regex(@"xx[-].*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        w = "header('#kvs') ~= 'xx[-].*'";
        _headersRegex = StatementParser.ConvertToFunc(w);
        _headersRegexV2 = HttpRoutingStatementParser.ConvertToFunction(w);
        //_t = f.Convert("{Path}#{{}}{Cookie('x-c')}");

        EqualStatement.EqualityComparers[typeof(PathString)] = (l, r) =>
        {
            if (l is PathString pl)
            {
                if (pl.HasValue)
                {
                    if (r is PathString pr)
                    {
                        return pl == pr;
                    }
                    else if (r is string rs)
                    {
                        return pl.Value == rs;
                    }
                }
                else
                    return false;
            }
            return false;
        };
        dcontext = new Lmzzz.Template.Inner.TemplateContext(HttpContext)
        {
            FieldMode = FieldStatementMode.Defined
        };
        //var fs = FieldStatement.getters.GetOrAdd("field_Request.Path", static k => new ConcurrentDictionary<Type, Func<object, object>>());
        //fs.GetOrAdd(typeof(DefaultHttpContext), t => (o) => ((HttpContext)o).Request.Path.Value);
    }

    public class ActionConditionStatement : IConditionStatement
    {
        public readonly Func<TemplateContext, bool> action;

        public ActionConditionStatement(Func<TemplateContext, bool> action)
        {
            this.action = action;
        }

        public object? Evaluate(TemplateContext context)
        {
            return EvaluateCondition(context);
        }

        public bool EvaluateCondition(TemplateContext context)
        {
            return action(context);
        }
    }

    [Benchmark(Baseline = true), BenchmarkCategory("PathEqual")]
    public void PathEqualString()
    {
        var b = string.Equals(HttpContext.Request.Path.Value, "/testp", StringComparison.OrdinalIgnoreCase);
        var c = string.Equals(HttpContext.Request.Path.Value, "/testp/DSD/fsdfx/fadasd3/中", StringComparison.OrdinalIgnoreCase);
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
        var b = _LmzzzEqual.EvaluateCondition(dcontext);
        var c = _LmzzzEqualTrue.EvaluateCondition(dcontext);
    }

    //[Benchmark(Baseline = true), BenchmarkCategory("Regx")]
    //public void PathRegxString()
    //{
    //    var b = regx.IsMatch(HttpContext.Request.Path.Value);
    //}

    //[Benchmark, BenchmarkCategory("Regx")]
    //public void PathRegx()
    //{
    //    var b = _PathRegx(HttpContext);
    //}

    //[Benchmark, BenchmarkCategory("Regx")]
    //public void PathRegxV2()
    //{
    //    var b = _PathRegxV2(HttpContext);
    //}

    //[Benchmark(Baseline = true), BenchmarkCategory("In")]
    //public void PathInString()
    //{
    //    var b = set.Contains(HttpContext.Request.Path.Value);
    //}

    //[Benchmark, BenchmarkCategory("In")]
    //public void PathIn()
    //{
    //    var b = _PathIn(HttpContext);
    //}

    //[Benchmark, BenchmarkCategory("In")]
    //public void PathInV2()
    //{
    //    var b = _PathInV2(HttpContext);
    //}

    //[Benchmark(Baseline = true), BenchmarkCategory("bool")]
    //public void IsHttps()
    //{
    //    var b = HttpContext.Request.IsHttps == true;
    //}

    //[Benchmark, BenchmarkCategory("bool")]
    //public void IsHttpsp()
    //{
    //    var b = _IsHttps(HttpContext);
    //}

    //[Benchmark, BenchmarkCategory("bool")]
    //public void IsHttpspV2()
    //{
    //    var b = _IsHttpsV2(HttpContext);
    //}

    [Benchmark(Baseline = true), BenchmarkCategory("Complex")]
    public void Complex()
    {
        var req = HttpContext.Request;
        var b = req.IsHttps == true
            && req.Path.Value.Equals("/testp/DSD/fsdfx/fadasd3/中", StringComparison.OrdinalIgnoreCase)
            && req.Method.Equals("GET", StringComparison.OrdinalIgnoreCase)
            && req.Host.ToString().Equals("x.com", StringComparison.OrdinalIgnoreCase)
            && req.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)
            && req.Protocol.Equals("HTTP/1.1", StringComparison.OrdinalIgnoreCase)
            && req.ContentType.Equals("json", StringComparison.OrdinalIgnoreCase)
            && queryRegx.IsMatch(req.QueryString.ToString())
            && !(req.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase));
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
        var b = _LmzzzPathComplex.EvaluateCondition(new TemplateContext(HttpContext));
    }

    //[Benchmark(Baseline = true), BenchmarkCategory("HeadersRegex")]
    //public void HeadersRegex()
    //{
    //    var req = HttpContext.Request;
    //    var b = req.Headers.Any(i => headersRegex.IsMatch(i.Key) || headersRegex.IsMatch(i.Value.ToString()));
    //}

    //[Benchmark, BenchmarkCategory("HeadersRegex")]
    //public void HeadersRegexp()
    //{
    //    var b = _headersRegex(HttpContext);
    //}

    //[Benchmark, BenchmarkCategory("HeadersRegex")]
    //public void HeadersRegexpV2()
    //{
    //    var b = _headersRegexV2(HttpContext);
    //}

    //[Benchmark(Baseline = true), BenchmarkCategory("Template")]
    //public void Template()
    //{
    //    var req = HttpContext.Request;
    //    var b = $"{req.Path}#{{}}{req.Cookies["x-c"]}".ToUpper();
    //}

    //[Benchmark, BenchmarkCategory("Template")]
    //public void TemplateF()
    //{
    //    var b = _t(HttpContext);
    //}
}