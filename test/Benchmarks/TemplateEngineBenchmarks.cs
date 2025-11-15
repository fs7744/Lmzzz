using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Fluid;
using Lmzzz;
using Lmzzz.Template;
using Scriban;

namespace Benchmarks;

[MemoryDiagnoser, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
public class TemplateEngineBenchmarks
{
    private A data = new A
    {
        Int = 4,
        D = 5.5,
        IntD = new Dictionary<string, int>() { { "a99", 44 }, { "99", 144 } },
        Array = [2, 34, 55],
        List = [2, 34, 55],
        LinkedList = new LinkedList<int>([2, 34, 55]) { },
        Str = "9sd"
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
        Lmzzz.Template.Inner.EqualStatement.EqualityComparers[typeof(Microsoft.AspNetCore.Http.PathString)] = (l, r) =>
        {
            if (l is Microsoft.AspNetCore.Http.PathString pl)
            {
                if (pl.HasValue)
                {
                    if (r is Microsoft.AspNetCore.Http.PathString pr)
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
        _ifcached = "{{ if(4 == Int)}}{{ if(5 == Int)}}{{ Int }}dd{{endif}} xx {{ if(4 == Int)}}{{ Int }}yy{{endif}}{{endif}}".ToTemplate();
        _ScribanIfCached = Template.Parse("{{ if int ==4;  if 5 == int ; $\"{int}dd xx \" ; end ;   if 4 == int ; $\" xx {int}yy\" ; end ;end; }}");
        var source = "{% if 4 == Int %} {% if 5 == Int %} {{ Int }}dd  xx {% elsif  4 == Int %} xx {{Int}}yy{% endif %}{% endif %}";
        f = new FluidParser();
        f.TryParse(source, out _FluidIfCached, out var error);

        _forCached = "{{ for(_v,_i in Array)}} {{_i}}:{{_v}},{{endfor}}".ToTemplate();
        _ScribanForCached = Template.Parse("{{ for $i in (0..(array.size-1)) }} {{$i}}:{{array[$i]}},{{end}}");

        source = "{% for i in Array %} {{forloop.index}}:{{i}},{% endfor %}";
        f.TryParse(source, out _FluidForCached, out error);
    }

    [Benchmark, BenchmarkCategory("if")]
    public string IfNoCache()
    {
        return "{{ if(4 == Int)}}{{ if(5 == Int)}}{{ Int }}dd{{endif}} xx {{ if(4 == Int)}}{{ Int }}yy{{endif}}{{endif}}".EvaluateTemplate(data);
    }

    [Benchmark, BenchmarkCategory("if")]
    public string IfCached()
    {
        return _ifcached.Evaluate(data);
    }

    [Benchmark, BenchmarkCategory("if")]
    public string ScribanIfNoCache()
    {
        var template = Template.Parse("{{ if int ==4;  if 5 == int ; $\"{int}dd xx \" ; end ;   if 4 == int ; $\" xx {int}yy\" ; end ;end; }}");
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
        var source = "{% if 4 == Int %} {% if 5 == Int %} {{ Int }}dd  xx {% elsif  4 == Int %} xx {{Int}}yy{% endif %}{% endif %}";
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
        public double? Dd;
        public double? D;
        public int Int { get; set; }
        public string Str { get; set; }
        public Dictionary<string, int> IntD { get; set; }

        public int[] Array { get; set; }

        public List<int> List { get; set; }

        public LinkedList<int> LinkedList { get; set; }
    }
}