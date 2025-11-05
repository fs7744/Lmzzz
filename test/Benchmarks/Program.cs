using BenchmarkDotNet.Running;
using Benchmarks;

//var a = new JsonPathBenchmarks();
//var aa = a.CacheTest();
//var aaa = a.NoCacheTest();
//var aaaa = a.NewtonsoftTest();
//var aaaaa = a.JsonPathNetTest();
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);