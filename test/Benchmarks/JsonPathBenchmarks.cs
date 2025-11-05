using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Json.Path;
using Lmzzz.Chars.Fluent;
using Lmzzz.JsonPath;
using System.Text.Json;
using System.Text.Json.Nodes;

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
    private readonly JsonPath pc;

    public JsonPathBenchmarks()
    {
        json = JsonSerializer.Serialize(data);
        JsonPathParser.Parser.TryParseResult(path, out var result, out var error);
        cache = result.Value;
        pc = JsonPath.Parse(path);
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

    [Benchmark]
    public object JsonPathNetTest()
    {
        var p = JsonPath.Parse(path);
        var instance = JsonNode.Parse(json);
        return p.Evaluate(instance);
    }

    [Benchmark]
    public object JsonPathNetCacheTest()
    {
        var instance = JsonNode.Parse(json);
        return pc.Evaluate(instance);
    }
}