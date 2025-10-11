namespace UT.Chars;

using Lmzzz.Chars;
using static Lmzzz.Chars.Parsers;

public class TermsTest
{
    [Fact]
    public void Char()
    {
        var t = Terms.Char('{');
        Assert.True(t.TryParse("{", out var c, out var err));
        Assert.Equal('{', c);
        Assert.Null(err);

        Assert.False(t.TryParse("}", out c, out err));
        Assert.Equal(Character.NullChar, c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n  {", out c, out err));
        Assert.Equal('{', c);
        Assert.Null(err);
    }

    [Fact]
    public void Text()
    {
        var t = Terms.Text("select", true);
        Assert.True(t.TryParse("select", out var c, out var err));
        Assert.Equal("select", c);
        Assert.Null(err);

        Assert.False(t.TryParse("se", out c, out err));
        Assert.Null(c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n  SeleCt ", out c, out err));
        Assert.Equal("select", c);
        Assert.Null(err);
    }
}