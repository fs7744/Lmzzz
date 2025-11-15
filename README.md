# Lmzzz

Lmzzz (Let me zzz) 是一个简单、轻量的辅助实现解析器的.NET 框架, 

让大家可以类函数或Fluent形式贴近 ABNF 格式实现语法定义。

（PS：目前暂时只支持 string解析，大多实现参考于[Parlot](https://github.com/sebastienros/parlot)）

## Getting Started

首先引入命名空间

``` csharp
using Lmzzz.Chars.Fluent;
using static Lmzzz.Chars.Fluent.Parsers;
```

然后就可以实现相关语法定义，如下是 一个完整的 json 解析器定义


``` csharp
using Lmzzz.Chars.Fluent;
using static Lmzzz.Chars.Fluent.Parsers;

namespace Samples.Json;

public class JsonParser
{
    public static readonly Parser<IJson> Json;

    static JsonParser()
    {
        var lBrace = IgnoreSeparator(Char('{'));
        var rBrace = IgnoreSeparator(Char('}'));
        var lBracket = IgnoreSeparator(Char('['));
        var rBracket = IgnoreSeparator(Char(']'));
        var colon = IgnoreSeparator(Char(':'));
        var comma = IgnoreSeparator(Char(','));
        var nullv = IgnoreSeparator(Text("null")).Then<IJson>(static s => JsonNull.Value);
        var boolv = IgnoreSeparator((Text("true", true)).Then<IJson>(static s => JsonBool.True))
                .Or(IgnoreSeparator(Text("false", true)).Then<IJson>(static s => JsonBool.False));

        var number = Decimal().Then<IJson>(static s => new JsonNumber(s));

        var str = String();

        var jsonString =
            str
                .Then<IJson>(static s => new JsonString(s.ToString()));

        var json = Deferred<IJson>();

        var jsonArray =
            Between(lBracket, Separated(comma, json), rBracket)
                .Then<IJson>(static els => new JsonArray(els));

        var jsonMember =
            str.And(colon).And(json)
                .Then(static member => new KeyValuePair<string, IJson>(member.Item1.ToString(), member.Item3));

        var jsonObject =
            Between(lBrace, Separated(comma, jsonMember), rBrace)
                .Then<IJson>(static kvps => new JsonObject(new Dictionary<string, IJson>(kvps)));

        Json = json.Parser = jsonString.Or(jsonArray).Or(jsonObject).Or(nullv).Or(boolv).Or(number);
        Json = Json.Eof();
    }

    public static IJson Parse(string input)
    {
        if (Json.TryParse(input, out var result, out _))
        {
            return result;
        }
        else
        {
            return null;
        }
    }
}
```

## 简单的模板引擎

为了展示解析器的方便，同时也实现了一个简单的模板引擎

### 对象取值

``` csharp
// data: {A: { Age : 15}, B: [ 2, 3, 4]}

// {{FieldName.FieldName}}
{{ A.Age }}  // result: 15

// index {{Array.Index}}
{{ B.0 }} // result: 2
{{ B.1 }} // result: 3
{{ B.99 }} // result: null

```

### if

``` csharp

{{ if(4 == Int || 5 != 6) }}
xx 
{{ elseif( true && false) }}
yy
{{ else }}
zz
{{endif}}

```

### for

``` csharp

{{  for(自定义值名,自定义索引名 in Array) }}

{{ 自定义值名 }}

{{endfor}}

```

## 简单性能展示

对于简单场景做了一些性能比较

#### 模板引擎

```

BenchmarkDotNet v0.15.7, Windows 11 (10.0.26100.6584/24H2/2024Update/HudsonValley)
Intel Core i7-10700 CPU 2.90GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method            | Mean        | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|------------------ |------------:|----------:|----------:|-------:|-------:|----------:|
| ForNoCache        |  5,386.7 ns | 104.63 ns |  92.75 ns | 0.4730 |      - |    3992 B |
| ForCached         |    446.1 ns |   2.23 ns |   1.98 ns | 0.1030 |      - |     864 B |
| ScribanForNoCache | 23,335.6 ns | 234.61 ns | 183.17 ns | 5.0049 | 0.3662 |   42439 B |
| ScribanForCached  | 13,213.1 ns | 251.76 ns | 279.83 ns | 4.2114 | 0.3662 |   35594 B |
| FluidForNoCache   |  3,423.2 ns |  19.39 ns |  18.14 ns | 0.3510 |      - |    2944 B |
| FluidForCached    |    961.4 ns |   4.18 ns |   3.71 ns | 0.1526 |      - |    1280 B |
|                   |             |           |           |        |        |           |
| IfNoCache         |  9,580.2 ns |  78.95 ns |  69.99 ns | 0.6714 |      - |    5736 B |
| IfCached          |    273.2 ns |   4.33 ns |   4.05 ns | 0.0763 |      - |     640 B |
| ScribanIfNoCache  | 20,510.4 ns | 363.77 ns | 533.21 ns | 4.8828 | 0.3662 |   41182 B |
| ScribanIfCached   | 11,424.6 ns | 157.37 ns | 147.20 ns | 4.0283 | 0.3662 |   33788 B |
| FluidIfNoCache    |  4,670.4 ns |  17.76 ns |  15.75 ns | 0.3738 |      - |    3168 B |
| FluidIfCached     |    405.3 ns |   3.13 ns |   2.93 ns | 0.0572 |      - |     480 B |


#### json 

```

BenchmarkDotNet v0.15.7, Windows 11 (10.0.26100.6584/24H2/2024Update/HudsonValley)
Intel Core i7-10700 CPU 2.90GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]   : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  ShortRun : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                  | Mean       | Error       | StdDev    | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
|------------------------ |-----------:|------------:|----------:|------:|--------:|--------:|-------:|----------:|------------:|
| BigJson_ParlotCompiled  | 125.121 μs |  16.0291 μs | 0.8786 μs |  1.00 |    0.01 | 11.2305 | 1.4648 |  91.76 KB |        1.00 |
| BigJson_Lmzzz           | 100.051 μs | 142.9565 μs | 7.8359 μs |  0.80 |    0.05 | 21.1182 | 3.1738 | 173.36 KB |        1.89 |
| BigJson_Parlot          | 126.344 μs |  38.5755 μs | 2.1145 μs |  1.01 |    0.02 | 11.2305 | 1.4648 |  91.76 KB |        1.00 |
| BigJson_Newtonsoft      |  97.092 μs |   5.1302 μs | 0.2812 μs |  0.78 |    0.01 | 24.7803 | 7.3242 |  203.1 KB |        2.21 |
| BigJson_SystemTextJson  |  19.048 μs |   3.2110 μs | 0.1760 μs |  0.15 |    0.00 |  2.9297 | 0.2747 |  24.12 KB |        0.26 |
|                         |            |             |           |       |         |         |        |           |             |
| DeepJson_ParlotCompiled |  53.866 μs |   8.5671 μs | 0.4696 μs |  1.00 |    0.01 | 12.0239 | 1.2817 |  98.32 KB |        1.00 |
| DeepJson_Lmzzz          |  71.322 μs |   9.0438 μs | 0.4957 μs |  1.32 |    0.01 | 15.1367 | 1.3428 | 124.37 KB |        1.26 |
| DeepJson_Parlot         |  52.478 μs |   8.7423 μs | 0.4792 μs |  0.97 |    0.01 | 12.0239 | 1.2817 |  98.32 KB |        1.00 |
| DeepJson_Newtonsoft     |  65.771 μs |  10.7962 μs | 0.5918 μs |  1.22 |    0.01 | 21.8506 | 6.4697 | 179.13 KB |        1.82 |
| DeepJson_SystemTextJson |  77.508 μs |   3.3456 μs | 0.1834 μs |  1.44 |    0.01 |  2.4414 | 0.1221 |  20.24 KB |        0.21 |
|                         |            |             |           |       |         |         |        |           |             |
| LongJson_ParlotCompiled |  86.733 μs |  24.4731 μs | 1.3415 μs |  1.00 |    0.02 | 14.4043 | 2.8076 | 118.34 KB |        1.00 |
| LongJson_Lmzzz          |  83.418 μs |  23.3307 μs | 1.2788 μs |  0.96 |    0.02 | 20.7520 | 4.1504 | 170.28 KB |        1.44 |
| LongJson_Parlot         |  83.721 μs |   8.0349 μs | 0.4404 μs |  0.97 |    0.01 | 14.4043 | 2.8076 | 118.34 KB |        1.00 |
| LongJson_Newtonsoft     |  81.515 μs |  19.5927 μs | 1.0739 μs |  0.94 |    0.02 | 24.7803 | 7.4463 | 202.68 KB |        1.71 |
| LongJson_SystemTextJson |  15.643 μs |   1.4569 μs | 0.0799 μs |  0.18 |    0.00 |  2.9297 | 0.2747 |  24.12 KB |        0.20 |
|                         |            |             |           |       |         |         |        |           |             |
| WideJson_ParlotCompiled |  55.410 μs |  31.9426 μs | 1.7509 μs |  1.00 |    0.04 |  4.9438 | 0.4883 |  40.55 KB |        1.00 |
| WideJson_Lmzzz          |  40.326 μs |  12.5477 μs | 0.6878 μs |  0.73 |    0.02 | 11.2915 | 1.0986 |   92.5 KB |        2.28 |
| WideJson_Parlot         |  53.288 μs |   2.2645 μs | 0.1241 μs |  0.96 |    0.03 |  4.9438 | 0.4883 |  40.55 KB |        1.00 |
| WideJson_Newtonsoft     |  47.165 μs |   0.5885 μs | 0.0323 μs |  0.85 |    0.02 | 13.0615 | 2.9297 | 106.72 KB |        2.63 |
| WideJson_SystemTextJson |   8.270 μs |   1.6550 μs | 0.0907 μs |  0.15 |    0.00 |  1.9684 | 0.0763 |  16.12 KB |        0.40 |