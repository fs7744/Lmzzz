using Lmzzz.Chars.Fluent;
using Lmzzz.JsonPath;
using System.Text.Json;

namespace UT.JsonPath;

public class JsonPathTest
{
    //[Theory]
    //[InlineData("{\"a\":33}")]
    //public void Test(string test)
    //{
    //    var g = JsonValue.Create(JsonDocument.Parse(test)) as JsonNode;
    //    var gk = g.GetValueKind();
    //    var d = JsonValue.Parse(test) as JsonNode;
    //    var dk = d.GetValueKind();
    //    var r = JsonValue.Create(new { a = 43 }) as JsonNode;
    //    var rk = r.GetValueKind();
    //}

    private object data = new
    {
        Num = -3.4,
        Array = new object[]
        {
            new { Name = "Alice", Age = 30 },
            new { Name = "Bob", Age = 25 },
            new { Name = "Charlie", Age = 35 }
        },
    };

    private string json;

    public JsonPathTest()
    {
        json = JsonSerializer.Serialize(data);
    }

    [Theory]
    [InlineData("$", "$")]
    [InlineData("$.Num", "-3.4")]
    [InlineData("$[\"Num\"]", "-3.4")]
    [InlineData("$['Num']", "-3.4")]
    [InlineData("$.Array[1]", "{\"Name\":\"Bob\",\"Age\":25}")]
    [InlineData("$.Array[0].Age", "30")]
    public void EvaluateJsonTest(string test, string r)
    {
        if (r == "$")
            r = json;
        Assert.True(JsonPathParser.Parser.TryParseResult(test, out var result, out var error));
        var s = result.Value;
        Assert.NotNull(s);
        var n = s.EvaluateJson(json);
        Assert.Equal(r, JsonSerializer.Serialize(n));
    }

    [Theory]
    [InlineData("$", "null")]
    public void EvaluateNullTest(string test, string r)
    {
        Assert.True(JsonPathParser.Parser.TryParseResult(test, out var result, out var error));
        var s = result.Value;
        Assert.NotNull(s);
        var n = s.EvaluateJson(null);
        Assert.Equal(r, JsonSerializer.Serialize(n));
    }

    [Theory]
    [InlineData("$", "null")]
    public void EvaluateObjectNullTest(string test, string r)
    {
        Assert.True(JsonPathParser.Parser.TryParseResult(test, out var result, out var error));
        var s = result.Value;
        Assert.NotNull(s);
        var n = s.EvaluateObject<object>(null);
        Assert.Equal(r, JsonSerializer.Serialize(n));
    }

    [Theory]
    [InlineData("$", "$")]
    public void EvaluateObjectTest(string test, string r)
    {
        if (r == "$")
            r = json;
        Assert.True(JsonPathParser.Parser.TryParseResult(test, out var result, out var error));
        var s = result.Value;
        Assert.NotNull(s);
        var n = s.EvaluateObject(data);
        Assert.Equal(r, JsonSerializer.Serialize(n));
    }
}