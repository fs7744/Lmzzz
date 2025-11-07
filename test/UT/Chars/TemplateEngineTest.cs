using Lmzzz;
using Lmzzz.Chars.Fluent;
using Lmzzz.Template.Inner;

namespace UT.Chars;

public class TemplateEngineTest
{
    [Theory]
    [InlineData("true == 1")]
    [InlineData("true")]
    [InlineData("1 != false")]
    [InlineData("false")]
    [InlineData("(true == 1)")]
    [InlineData("(true)")]
    [InlineData("(1 != false)")]
    [InlineData("(false)")]
    [InlineData("(false) and true")]
    [InlineData("1 != 2.3")]
    [InlineData("2 > s")]
    [InlineData("(s > f(2))")]
    [InlineData("1 != 2.3 and 2 > s.d.d")]
    [InlineData("1 != 2.3 and (2 > s.d.d and false)")]
    [InlineData("1 != 2.3 or 2 > s.d.d")]
    [InlineData("(1 != 2.3 or (2 > s.d.d and 6 != 7)) and 3 !=4")]
    [InlineData("(1 != f(2.3) or (2 > s.d.d and 6 != 7)) and 3 !=4")]
    [InlineData("(1 != f(2.3) && z() || true || f(s.d))")]
    [InlineData("! (1 != f(2.3) && z() || true || f(s.d))")]
    public void ConditionParserTest(string text)
    {
        var r = TemplateEngineParser.ConditionParser.TryParse(text, out var v, out var err);
        Assert.True(r);
        Assert.NotNull(v);
    }

    [Theory]
    [InlineData("f(2)")]
    [InlineData("f()")]
    [InlineData("f")]
    public void AnyValueTest(string text)
    {
        var r = TemplateEngineParser.AnyValue.Eof().TryParse(text, out var v, out var err);
        Assert.True(r);
        Assert.NotNull(v);
    }

    [Theory]
    [InlineData("s > f(2)")]
    public void OPTest(string text)
    {
        var r = TemplateEngineParser.OP.Eof().TryParse(text, out var v, out var err);
        Assert.True(r);
        Assert.NotNull(v);
    }

    [Theory]
    [InlineData("f(2)")]
    [InlineData("f(s.d)")]
    [InlineData("z()")]
    public void ConditionValueTest(string text)
    {
        var r = TemplateEngineParser.ConditionValue.Eof().TryParse(text, out var v, out var err);
        Assert.True(r);
        Assert.NotNull(v);
    }

    [Theory]
    [InlineData("true == 1 and [] !=s")]
    public void ConditionParserError(string text)
    {
        var r = TemplateEngineParser.ConditionParser.TryParse(text, out var v, out var err);
        Assert.False(r);
        Assert.NotNull(err);
        Assert.NotNull(err.Position.ToString());
    }
}