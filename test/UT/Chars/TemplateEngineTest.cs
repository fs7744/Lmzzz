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

    private A data = new A { Int = 4, D = 5.5, IntD = new Dictionary<string, int>() { { "a99", 44 } } };

    [Theory]
    [InlineData("Int", 4)]
    [InlineData("D", 5.5)]
    [InlineData("IntD.a99", 44)]
    public void FieldTest(string text, object d)
    {
        var r = TemplateEngineParser.Field.Eof().TryParse(text, out var v, out var err);
        Assert.True(r);
        var dd = new TemplateContext(data);
        var f = v.Evaluate(dd);
        Assert.Equal(d, f);
    }

    [Theory]
    [InlineData("4 == Int", true)]
    [InlineData("Int == 4", true)]
    [InlineData("D == null", false)]
    [InlineData("D == 5.5", true)]
    [InlineData("Dd == 5.5", false)]
    [InlineData("Dd == Dd", true)]
    [InlineData("4 != Int", false)]
    [InlineData("Int != 4", false)]
    [InlineData("D != null", true)]
    [InlineData("D != 5.5", false)]
    [InlineData("Dd != 5.5", true)]
    [InlineData("Dd != Dd", false)]
    [InlineData("D >= 5.5", true)]
    [InlineData("D > 5.5", false)]
    [InlineData("D <= 5.5", true)]
    [InlineData("D < 5.5", false)]
    [InlineData("!(D < 5.5)", true)]
    [InlineData("!(null == null)", false)]
    [InlineData("!(null != null)", true)]
    [InlineData("!(null != null) and 1 == 3", false)]
    [InlineData("!(null != null) && 1 == 3", false)]
    [InlineData("!(null != null) || 1 == 3", true)]
    [InlineData("!(null != null) or 1 == 3", true)]
    public void ConditionEvaluateTest(string text, object d)
    {
        var r = TemplateEngineParser.ConditionParser.TryParse(text, out var v, out var err);
        Assert.True(r);
        Assert.NotNull(v);
        var dd = new TemplateContext(data);
        var f = v.Evaluate(dd);
        Assert.Equal(d, f);
    }
}

public class A
{
    public double? Dd;
    public double? D;
    public int Int { get; set; }

    public Dictionary<string, int> IntD { get; set; }
}