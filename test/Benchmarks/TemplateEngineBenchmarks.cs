using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Fluid;
using Lmzzz;
using Lmzzz.Template;
using Scriban;

namespace Benchmarks;

[MemoryDiagnoser, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
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
    private readonly FluidParser f;

    public TemplateEngineBenchmarks()
    {
        _ifcached = "{{ if(4 == Int)}}{{ if(5 == Int)}}{{ Int }}dd{{endif}} xx {{ if(4 == Int)}}{{ Int }}yy{{endif}}{{endif}}".ToTemplate();
        _ScribanIfCached = Template.Parse("{{ if int ==4;  if 5 == int ; $\"{int}dd xx \" ; end ;   if 4 == int ; $\" xx {int}yy\" ; end ;end; }}");
        var source = "{% if 4 == Int %} {% if 5 == Int %} {{ Int }}dd  xx {% elsif  4 == Int %} xx {{Int}}yy{% endif %}{% endif %}";
        f = new FluidParser();
        f.TryParse(source, out _FluidIfCached, out var error);
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