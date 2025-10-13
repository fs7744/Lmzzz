using Samples.Json;

namespace UT.Chars;

public class JsonParserTest
{
    [Theory]
    [InlineData("{\"key\":\"value\"}")]
    [InlineData("{\"key\":True}")]
    [InlineData("{\"key\":False}")]
    [InlineData("{\"key\":null}")]
    [InlineData("{\"key\":1.23}")]
    [InlineData("{\"key\":-0.23}")]
    public void JsonTest(string json)
    {
        var v = JsonParser.Parse(json);
        Assert.NotNull(v);
        Assert.Equal(json, v.ToString());
    }
}