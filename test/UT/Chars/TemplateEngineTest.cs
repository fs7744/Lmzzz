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
    [InlineData("1 != 2.3 and 2 > s.d.d")]
    [InlineData("1 != 2.3 and (2 > s.d.d and false)")]
    [InlineData("1 != 2.3 or 2 > s.d.d")]
    [InlineData("(1 != 2.3 or (2 > s.d.d and 6 != 7)) and 3 !=4")]
    public void ConditionParserTest(string text)
    {
        var r = TemplateEngineParser.ConditionParser.TryParse(text, out var v, out var err);
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