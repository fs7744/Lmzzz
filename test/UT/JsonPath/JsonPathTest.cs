using Lmzzz.Chars.Fluent;
using Lmzzz.JsonPath;
using Lmzzz.JsonPath.Statements;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

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
        Num = -3.4
    };

    private string json;

    public JsonPathTest()
    {
        json = JsonSerializer.Serialize(data);
    }

    [Theory]
    [InlineData("$", "{\"Num\":-3.4}")]
    [InlineData("$.Num", "-3.4")]
    [InlineData("$[\"Num\"]", "-3.4")]
    [InlineData("$['Num']", "-3.4")]
    public void EvaluateJsonTest(string test, string r)
    {
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
    [InlineData("$", "{\"Num\":-3.4}")]
    public void EvaluateObjectTest(string test, string r)
    {
        Assert.True(JsonPathParser.Parser.TryParseResult(test, out var result, out var error));
        var s = result.Value;
        Assert.NotNull(s);
        var n = s.EvaluateObject(data);
        Assert.Equal(r, JsonSerializer.Serialize(n));
    }
}