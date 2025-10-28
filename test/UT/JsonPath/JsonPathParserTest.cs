using Lmzzz.JsonPath;
using Lmzzz.Chars.Fluent;
using Lmzzz.JsonPath.Statements;

namespace UT.JsonPath;

public class JsonPathParserTest
{
    [Theory]
    [InlineData("sss", true, "sss")]
    [InlineData("sss.s", true, "sss")]
    public void MemberNameShorthandTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.MemberNameShorthand;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        Assert.Equal(rr, (v as Member).Name);
    }
}