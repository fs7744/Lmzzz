using Lmzzz.JsonPath;
using Lmzzz.Chars.Fluent;

namespace UT.JsonPath;

public class JsonPathParserTest
{
    [Theory]
    [InlineData("sss", true)]
    public void NameFirstTest(string test, bool r)
    {
        var p = JsonPathParser.NameFirst;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
    }
}