using Lmzzz.JsonPath;
using Lmzzz.Chars.Fluent;
using Lmzzz.JsonPath.Statements;

namespace UT.JsonPath;

public class JsonPathParserTest
{
    [Theory]
    [InlineData("sss", true, "sss")]
    [InlineData("sss.s", true, "sss")]
    [InlineData("dsd[sd]csd", true, "dsd")]
    [InlineData("[sd]csd", false, "")]
    public void MemberNameShorthandTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.MemberNameShorthand;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, (v as Member).Name);
    }

    [Theory]
    [InlineData("1.23", true, 1.23)]
    [InlineData("-0.55", true, -0.55)]
    [InlineData("9e-4", true, 0.0009)]
    [InlineData("[sd]csd", false, 0)]
    [InlineData("--3", false, 0)]
    [InlineData("+3", false, 0)]
    public void NumTest(string test, bool r, decimal rr)
    {
        var p = JsonPathParser.Num;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, (v as NumberValue).Value);
    }

    [Theory]
    [InlineData("123", true, 123)]
    [InlineData("1.23", true, 1)]
    [InlineData("-0.55", true, -0)]
    [InlineData("9e-4", true, 9)]
    [InlineData("[sd]csd", false, 0)]
    [InlineData("--3", false, 0)]
    [InlineData("+3", false, 0)]
    public void IntTest(string test, bool r, decimal rr)
    {
        var p = JsonPathParser.Int;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, v);
    }

    [Theory]
    [InlineData("\"ss\r\ns\"", true, "ss\r\ns")]
    [InlineData("\"sss\"", true, "sss")]
    [InlineData("'sss'", true, "sss")]
    [InlineData("sss", false, "")]
    public void StringLiteralTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.StringLiteral;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, v.Span.ToString());
    }

    [Theory]
    [InlineData("\"ss\r\ns\"", true, "ss\r\ns")]
    [InlineData("\"sss\"", true, "sss")]
    [InlineData("'sss'", true, "sss")]
    [InlineData("true", true, true)]
    [InlineData("false", true, false)]
    [InlineData("null", true, null)]
    [InlineData("sss", false, "")]
    public void LiteralTest(string test, bool r, object rr)
    {
        var p = JsonPathParser.Literal;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, (v as IStatementValue).Value);
    }

    [Theory]
    [InlineData("$", true, "$")]
    [InlineData("sss", false, "")]
    public void RootIdentifierTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.RootIdentifier;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, v.ToString());
    }

    [Theory]
    [InlineData("@", true, "@")]
    [InlineData("sss", false, "")]
    public void CurrentNodeIdentifierTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.CurrentNodeIdentifier;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, v.ToString());
    }

    [Theory]
    [InlineData("==", true, "==")]
    [InlineData("!=", true, "!=")]
    [InlineData("<=", true, "<=")]
    [InlineData("<", true, "<")]
    [InlineData(">=", true, ">=")]
    [InlineData(">", true, ">")]
    [InlineData("sss", false, "")]
    public void ComparisonOpTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.ComparisonOp;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, v);
    }
}