using BenchmarkDotNet.Running;
using Benchmarks;

//var a = new TemplateEngineBenchmarks();
//var aa = a.ScribanIfNoCache();
//var aaa = a.ScribanIfCached();
//var aaaa = a.IfCached();
//var aaaaa = a.IfNoCache();
//var aaaaf = a.IfCachedWhenFieldDefined();
//var aaaaaf = a.IfNoCacheWhenFieldDefined();
//var f = a.FluidIfNoCache();
//var ff = a.FluidIfCached();
var a = new PipeReadBufferStateBenchmarks();
for (int i = 0; i < 10; i++)
    a.IgnoreCaseTest();
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);