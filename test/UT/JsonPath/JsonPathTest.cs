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
        Nu = null as string,
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
    [InlineData("$.Nu.xx", "null")]
    [InlineData("$.Nu", "null")]
    [InlineData("$.Num", "-3.4")]
    [InlineData("$[\"Num\"]", "-3.4")]
    [InlineData("$['Num']", "-3.4")]
    [InlineData("$.Array[1]", "{\"Name\":\"Bob\",\"Age\":25}")]
    [InlineData("$.Array[0].Age", "30")]
    [InlineData("$.Array[-10].Age", "null")]
    [InlineData("$.Array[-1].Age", "35")]
    [InlineData("$.Array[2].Age", "35")]
    [InlineData("$.*", "[-3.4,null,[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]]")]
    [InlineData("$.*.*", "[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]")]
    [InlineData("$.*.*.*", "[\"Alice\",30,\"Bob\",25,\"Charlie\",35]")]
    [InlineData("$.*.*.*.*", "[]")]
    [InlineData("$.Array[0:1]", "[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25}]")]
    [InlineData("$.Array[0:1:2]", "[{\"Name\":\"Alice\",\"Age\":30}]")]
    [InlineData("$.Array[:1:2]", "[{\"Name\":\"Alice\",\"Age\":30}]")]
    [InlineData("$.Array[::]", "[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]")]
    [InlineData("$.Array[-5::]", "[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]")]
    [InlineData("$.Array[-5:-5:]", "[]")]
    [InlineData("$.Array[::-1]", "[]")]
    [InlineData("$.Array[::0]", "[]")]
    [InlineData("$.Array[-5:5:]", "[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]")]
    [InlineData("$.Array[::].Age", "[30,25,35]")]
    [InlineData("$[ 'Nu' , 'Num' ]", "[-3.4]")]
    [InlineData("$['Num','Array'].Age", "[]")]
    [InlineData("$.Array.*['Name','Age']", "[\"Alice\",30,\"Bob\",25,\"Charlie\",35]")]
    [InlineData("$.Array[?25==@.Age]", "[{\"Name\":\"Bob\",\"Age\":25}]")]
    [InlineData("$[?!(@==-3.4)]", "[null,[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]]")]
    [InlineData("$[?((@==-3.4))]", "[-3.4]")]
    [InlineData("$[?@==-3.4]", "[-3.4]")]
    [InlineData("$[?@<=-3.4]", "[-3.4]")]
    [InlineData("$[?@<-3.1]", "[-3.4]")]
    [InlineData("$[?@>-13.4]", "[-3.4]")]
    [InlineData("$[?@>=-3.4]", "[-3.4]")]
    [InlineData("$[?@!=-3.4]", "[null,[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]]")]
    [InlineData("$.Array[?(@.Age == 25.0)]", "[{\"Name\":\"Bob\",\"Age\":25}]")]
    [InlineData("$.Array[?(@.Age != 25 && @.Name != 'Alice')]", "[{\"Name\":\"Charlie\",\"Age\":35}]")]
    [InlineData("$.Array[?(@.Age != 25 || @.Name != 'Alice')]", "[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]")]
    [InlineData("$[?length(@) >= 3].*.Age", "[30,25,35]")]
    [InlineData("$[?count(@) >= 1].*.Age", "[30,25,35]")]
    [InlineData("$.Array[?value(@.Age) ==25]", "[{\"Name\":\"Bob\",\"Age\":25}]")]
    [InlineData("$.Array[?search(@.Name,'^B.*')]", "[{\"Name\":\"Bob\",\"Age\":25}]")]
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
    [InlineData("$.Nu.xx", "null")]
    [InlineData("$.Nu", "null")]
    [InlineData("$.Num", "-3.4")]
    [InlineData("$[\"Num\"]", "-3.4")]
    [InlineData("$['Num']", "-3.4")]
    [InlineData("$.Array[1]", "{\"Name\":\"Bob\",\"Age\":25}")]
    [InlineData("$.Array[0].Age", "30")]
    [InlineData("$.Array[-10].Age", "null")]
    [InlineData("$.Array[-1].Age", "35")]
    [InlineData("$.Array[2].Age", "35")]
    [InlineData("$.*", "[-3.4,null,[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]]")]
    [InlineData("$.*.*", "[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]")]
    [InlineData("$.*.*.*", "[\"Alice\",30,\"Bob\",25,\"Charlie\",35]")]
    [InlineData("$.*.*.*.*", "[]")]
    [InlineData("$.Array[0:1]", "[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25}]")]
    [InlineData("$.Array[0:1:2]", "[{\"Name\":\"Alice\",\"Age\":30}]")]
    [InlineData("$.Array[:1:2]", "[{\"Name\":\"Alice\",\"Age\":30}]")]
    [InlineData("$.Array[::]", "[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]")]
    [InlineData("$.Array[-5::]", "[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]")]
    [InlineData("$.Array[-5:-5:]", "[]")]
    [InlineData("$.Array[::-1]", "[]")]
    [InlineData("$.Array[::0]", "[]")]
    [InlineData("$.Array[-5:5:]", "[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]")]
    [InlineData("$.Array[::].Age", "[30,25,35]")]
    [InlineData("$[ 'Nu' , 'Num' ]", "[-3.4]")]
    [InlineData("$['Num','Array'].Age", "[]")]
    [InlineData("$.Array.*['Name','Age']", "[\"Alice\",30,\"Bob\",25,\"Charlie\",35]")]
    [InlineData("$.Array[?25==@.Age]", "[{\"Name\":\"Bob\",\"Age\":25}]")]
    [InlineData("$[?!(@==-3.4)]", "[null,[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]]")]
    [InlineData("$[?((@==-3.4))]", "[-3.4]")]
    [InlineData("$[?@==-3.4]", "[-3.4]")]
    [InlineData("$[?@<=-3.4]", "[-3.4]")]
    [InlineData("$[?@<-3.1]", "[-3.4]")]
    [InlineData("$[?@>-13.4]", "[-3.4]")]
    [InlineData("$[?@>=-3.4]", "[-3.4]")]
    [InlineData("$[?@!=-3.4]", "[null,[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]]")]
    [InlineData("$.Array[?(@.Age == 25.0)]", "[{\"Name\":\"Bob\",\"Age\":25}]")]
    [InlineData("$.Array[?(@.Age != 25 && @.Name != 'Alice')]", "[{\"Name\":\"Charlie\",\"Age\":35}]")]
    [InlineData("$.Array[?(@.Age != 25 || @.Name != 'Alice')]", "[{\"Name\":\"Alice\",\"Age\":30},{\"Name\":\"Bob\",\"Age\":25},{\"Name\":\"Charlie\",\"Age\":35}]")]
    [InlineData("$[?length(@) >= 3].*.Age", "[30,25,35]")]
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