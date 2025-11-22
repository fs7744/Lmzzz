using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Fluid;
using Lmzzz;
using Lmzzz.Template;
using Microsoft.AspNetCore.Http;
using Scriban;

namespace Benchmarks;

[MemoryDiagnoser, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
public class TemplateEngineBenchmarks
{
    private A data = new A
    {
        Int = 4,
        Array = [2, 34, 55],
    };

    private IStatement _ifcached;
    private Template _ScribanIfCached;
    private IFluidTemplate _FluidIfCached;
    private IStatement _forCached;
    private IFluidTemplate _FluidForCached;
    private readonly Template _ScribanForCached;
    private readonly FluidParser f;

    public TemplateEngineBenchmarks()
    {
        //Lmzzz.Template.Inner.EqualStatement.EqualityComparers[typeof(Microsoft.AspNetCore.Http.PathString)] = (l, r) =>
        //{
        //    if (l is Microsoft.AspNetCore.Http.PathString pl)
        //    {
        //        if (pl.HasValue)
        //        {
        //            if (r is Microsoft.AspNetCore.Http.PathString pr)
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
        _ifcached = "{{ if(4 == Int)}}{{ if(5 == Int)}}{{ HttpContext.Request.ContentType }}dd{{endif}} xx {{ if(4 == Int)}}{{ HttpContext.Request.ContentType }}yy{{endif}}{{endif}}".ToTemplate();
        _ScribanIfCached = Template.Parse("{{ if int ==4;  if 5 == int ; $\"{httpContext.Request.ContentType}dd xx \" ; end ;   if 4 == int ; $\" xx {httpContext}yy\" ; end ;end; }}");
        var source = "{% if 4 == Int %} {% if 5 == Int %} {{ HttpContext.Request.ContentType }}dd  xx {% elsif  4 == Int %} xx {{HttpContext.Request.ContentType}}yy{% endif %}{% endif %}";
        f = new FluidParser();
        f.TryParse(source, out _FluidIfCached, out var error);

        _forCached = "{{ for(_v,_i in Array)}} {{_i}}:{{_v}},{{endfor}}".ToTemplate();
        _ScribanForCached = Template.Parse("{{ for $i in (0..(array.size-1)) }} {{$i}}:{{array[$i]}},{{end}}");

        source = "{% for i in Array %} {{forloop.index}}:{{i}},{% endfor %}";
        f.TryParse(source, out _FluidForCached, out error);
        data.HttpContext = new DefaultHttpContext();
        var req = data.HttpContext.Request;
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

    [Benchmark, BenchmarkCategory("if")]
    public string IfNoCache()
    {
        return "{{ if(4 == Int)}}{{ if(5 == Int)}}{{ HttpContext.Request.ContentType }}dd{{endif}} xx {{ if(4 == Int)}}{{ HttpContext.Request.ContentType }}yy{{endif}}{{endif}}".EvaluateTemplate(data);
    }

    [Benchmark, BenchmarkCategory("if")]
    public string IfNoCacheWhenFieldDefined()
    {
        return "{{ if(4 == Int)}}{{ if(5 == Int)}}{{ HttpContext.Request.ContentType }}dd{{endif}} xx {{ if(4 == Int)}}{{ HttpContext.Request.ContentType }}yy{{endif}}{{endif}}".EvaluateTemplate(data, FieldStatementMode.Defined);
    }

    [Benchmark, BenchmarkCategory("if")]
    public string IfCached()
    {
        return _ifcached.Evaluate(data);
    }

    [Benchmark, BenchmarkCategory("if")]
    public string IfCachedWhenFieldDefined()
    {
        return _ifcached.Evaluate(data, FieldStatementMode.Defined);
    }

    [Benchmark, BenchmarkCategory("if")]
    public string ScribanIfNoCache()
    {
        var template = Template.Parse("{{ if int ==4;  if 5 == int ; $\"{httpcontext.request.contenttype}dd xx \" ; end ;   if 4 == int ; $\" xx {httpContext}yy\" ; end ;end; }}");
        return template.Render(data);
    }

    [Benchmark, BenchmarkCategory("if")]
    public string ScribanIfCached()
    {
        return _ScribanIfCached.Render(data);
    }

    [Benchmark, BenchmarkCategory("if")]
    public string FluidIfNoCache()
    {
        var source = "{% if 4 == Int %} {% if 5 == Int %} {{ HttpContext.Request.ContentType }}dd  xx {% elsif  4 == Int %} xx {{HttpContext.Request.ContentType}}yy{% endif %}{% endif %}";
        if (f.TryParse(source, out var template, out var error))
        {
            var context = new Fluid.TemplateContext(data);

            return template.Render(context);
        }
        else
        {
            return error;
        }
    }

    [Benchmark, BenchmarkCategory("if")]
    public string FluidIfCached()
    {
        var context = new Fluid.TemplateContext(data);

        return _FluidIfCached.Render(context);
    }

    [Benchmark, BenchmarkCategory("for")]
    public string ForNoCache()
    {
        return "{{ for(_v,_i in Array)}} {{_i}}:{{_v}},{{endfor}}".EvaluateTemplate(data);
    }

    [Benchmark, BenchmarkCategory("for")]
    public string ForCached()
    {
        return _forCached.Evaluate(data);
    }

    [Benchmark, BenchmarkCategory("for")]
    public string ScribanForNoCache()
    {
        var template = Template.Parse("{{ for $i in (0..(array.size-1)) }} {{$i}}:{{array[$i]}},{{end}}");
        return template.Render(data);
    }

    [Benchmark, BenchmarkCategory("for")]
    public string ScribanForCached()
    {
        return _ScribanForCached.Render(data);
    }

    [Benchmark, BenchmarkCategory("for")]
    public string FluidForNoCache()
    {
        var source = "{% for i in Array %} {{forloop.index}}:{{i}},{% endfor %}";
        if (f.TryParse(source, out var template, out var error))
        {
            var context = new Fluid.TemplateContext(data);

            return template.Render(context);
        }
        else
        {
            return error;
        }
    }

    [Benchmark, BenchmarkCategory("for")]
    public string FluidForCached()
    {
        var context = new Fluid.TemplateContext(data);

        return _FluidForCached.Render(context);
    }

    public class A
    {
        public int Int { get; set; }
        public HttpContext HttpContext { get; set; }
        public int[] Array { get; set; }
    }
}