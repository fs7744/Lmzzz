using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Lmzzz.Chars.Fluent;
using Lmzzz.JsonPath;
using System.Text.Json;

namespace Benchmarks;

[MemoryDiagnoser, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class JsonPathBenchmarks
{
    private object data = new
    {
        Num = -3.4,
        Nu = null as string,
        Array = new object[]
        {
            new { Name = "Alice", Age = 30 },
            new { Name = "Bob", Age = 25 },
            new { Name = "Charlie", Age = 35 }
        },
    };

    private string path = "$.Array[1]['Name','Age']";

    private string json;
    private IStatement cache;

    public JsonPathBenchmarks()
    {
        json = JsonSerializer.Serialize(data);
        JsonPathParser.Parser.TryParseResult(path, out var result, out var error);
        cache = result.Value;
    }

    [Benchmark]
    public object CacheTest()
    {
        return cache.EvaluateJson(json);
    }

    [Benchmark]
    public object NoCacheTest()
    {
        JsonPathParser.Parser.TryParseResult(path, out var result, out var error);
        return result.Value.EvaluateJson(json);
    }

    [Benchmark]
    public object NewtonsoftTest()
    {
        Newtonsoft.Json.Linq.JToken token = Newtonsoft.Json.Linq.JToken.Parse(json);
        return token.SelectTokens(path);
    }
}