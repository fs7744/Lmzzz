using BenchmarkDotNet.Running;
using Benchmarks;

var a = new TemplateEngineBenchmarks();
var aa = a.ForNoCache();
var aaa = a.ForCached();
var aaaa = a.IfCached();
var aaaaa = a.IfNoCache();
var f = a.FluidForNoCache();
var ff = a.FluidForCached();
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);