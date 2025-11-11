using BenchmarkDotNet.Running;
using Benchmarks;

//var a = new TemplateEngineBenchmarks();
//var aa = a.IfNoCache();
//var aaa = a.IfCached();
//var aaaa = a.ScribanIfNoCache();
//var aaaaa = a.ScribanIfCached();
//var f = a.FluidIfNoCache();
//var ff = a.FluidIfCached();
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);