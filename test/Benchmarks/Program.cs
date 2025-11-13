using BenchmarkDotNet.Running;
using Benchmarks;

//var a = new TemplateEngineBenchmarks();
//var aa = a.ForNoCache();
//var aaa = a.ForCached();
//var aaaa = a.ScribanForNoCache();
//var aaaaa = a.ScribanForCached();
//var f = a.FluidForNoCache();
//var ff = a.FluidForCached();
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);