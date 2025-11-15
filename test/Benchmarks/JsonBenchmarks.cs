using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Samples.Json;
using System.Text.Json;

namespace Benchmarks;

[MemoryDiagnoser, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), ShortRunJob]
public class JsonBenchmarks
{
    private string bigJson;
    private string longJson;
    private string wideJson;
    private string deepJson;
    private Parlot.Fluent.Parser<IJson> compiledParlot;
    private static readonly JsonSerializerSettings jsonSerializerSettings = new() { MaxDepth = 1024 };
    private static readonly JsonDocumentOptions jsonDocumentOptions = new() { MaxDepth = 1024 };

    [GlobalSetup]
    public void Setup()
    {
        bigJson = BuildJson(4, 4, 3).ToString()!;
        longJson = BuildJson(256, 1, 1).ToString()!;
        wideJson = BuildJson(1, 1, 256).ToString()!;
        deepJson = BuildJson(1, 256, 1).ToString()!;

        compiledParlot = Parlot.Tests.Json.JsonParser.Json.Compile();
    }

    private static IJson BuildJson(int length, int depth, int width)
       => new JsonArray(
           Enumerable.Repeat(1, length)
               .Select(_ => BuildObject(depth, width))
               .ToArray()
       );

    private static IJson BuildObject(int depth, int width)
    {
        if (depth == 0)
        {
            return new JsonString(RandomString(6));
        }
        return new JsonObject(
            new Dictionary<string, IJson>(
                Enumerable.Repeat(1, width)
                .Select(_ => new KeyValuePair<string, IJson>(RandomString(5), BuildObject(depth - 1, width)))
                )
        );
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }

    [Benchmark(Baseline = true), BenchmarkCategory("Big")]
    public IJson BigJson_ParlotCompiled()
    {
        return compiledParlot.Parse(bigJson);
    }

    [Benchmark, BenchmarkCategory("Big")]
    public IJson BigJson_Lmzzz()
    {
        return JsonParser.Parse(bigJson);
    }

    [Benchmark, BenchmarkCategory("Big")]
    public IJson BigJson_Parlot()
    {
        return Parlot.Tests.Json.JsonParser.Parse(bigJson);
    }

    [Benchmark, BenchmarkCategory("Big")]
    public JToken BigJson_Newtonsoft()
    {
        return JToken.Parse(bigJson);
    }

    [Benchmark, BenchmarkCategory("Big")]
    public JsonDocument BigJson_SystemTextJson()
    {
        return JsonDocument.Parse(bigJson);
    }

    [Benchmark(Baseline = true), BenchmarkCategory("Long")]
    public IJson LongJson_ParlotCompiled()
    {
        return compiledParlot.Parse(longJson);
    }

    [Benchmark, BenchmarkCategory("Long")]
    public IJson LongJson_Lmzzz()
    {
        return JsonParser.Parse(longJson);
    }

    [Benchmark, BenchmarkCategory("Long")]
    public IJson LongJson_Parlot()
    {
        return Parlot.Tests.Json.JsonParser.Parse(longJson);
    }

    [Benchmark, BenchmarkCategory("Long")]
    public JToken LongJson_Newtonsoft()
    {
        return JToken.Parse(longJson);
    }

    [Benchmark, BenchmarkCategory("Long")]
    public JsonDocument LongJson_SystemTextJson()
    {
        return JsonDocument.Parse(longJson);
    }

    [Benchmark(Baseline = true), BenchmarkCategory("Deep")]
    public IJson DeepJson_ParlotCompiled()
    {
        return compiledParlot.Parse(deepJson);
    }

    [Benchmark, BenchmarkCategory("Deep")]
    public IJson DeepJson_Lmzzz()
    {
        return JsonParser.Parse(deepJson);
    }

    [Benchmark, BenchmarkCategory("Deep")]
    public IJson DeepJson_Parlot()
    {
        return Parlot.Tests.Json.JsonParser.Parse(deepJson);
    }

    [Benchmark, BenchmarkCategory("Deep")]
    public JToken DeepJson_Newtonsoft()
    {
        return JsonConvert.DeserializeObject<JToken>(deepJson, jsonSerializerSettings);
    }

    [Benchmark, BenchmarkCategory("Deep")]
    public JsonDocument DeepJson_SystemTextJson()
    {
        return JsonDocument.Parse(deepJson, jsonDocumentOptions);
    }

    [Benchmark(Baseline = true), BenchmarkCategory("Wide")]
    public IJson WideJson_ParlotCompiled()
    {
        return compiledParlot.Parse(wideJson);
    }

    [Benchmark, BenchmarkCategory("Wide")]
    public IJson WideJson_Lmzzz()
    {
        return JsonParser.Parse(wideJson);
    }

    [Benchmark, BenchmarkCategory("Wide")]
    public IJson WideJson_Parlot()
    {
        return Parlot.Tests.Json.JsonParser.Parse(wideJson);
    }

    [Benchmark, BenchmarkCategory("Wide")]
    public JToken WideJson_Newtonsoft()
    {
        return JToken.Parse(wideJson);
    }

    [Benchmark, BenchmarkCategory("Wide")]
    public JsonDocument WideJson_SystemTextJson()
    {
        return JsonDocument.Parse(wideJson, jsonDocumentOptions);
    }
}