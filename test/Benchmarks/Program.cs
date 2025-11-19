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
var a = new Path_HttpRoutingStatementParserBenchmarks();
a.LmzzzRegx();
a.PathInV2();
a.LmzzzPathIn();
a.PathInString();
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);