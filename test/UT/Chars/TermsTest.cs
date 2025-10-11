namespace UT.Chars;

using Lmzzz.Chars;
using static Lmzzz.Chars.Parsers;

public class TermsTest
{
    [Fact]
    public void CharTest()
    {
        var t = Char('{').Eof();
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
    public void TextTest()
    {
        var t = Text("select", true).Eof();
        Assert.True(t.TryParse("select", out var c, out var err));
        Assert.Equal("select", c);
        Assert.Null(err);

        Assert.False(t.TryParse("se", out c, out err));
        Assert.Null(c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n  SeleCt", out c, out err));
        Assert.Equal("select", c);
        Assert.Null(err);

        Assert.False(t.TryParse(" \r\n  SeleCt ", out c, out err));
        Assert.Null(c);
        Assert.Null(err);
    }

    [Fact]
    public void AnyTest()
    {
        var t = Any("{{", true);
        Assert.True(t.TryParse(" \r\n   xada/l;fslffp{salfas;f{{", out var c, out var err));
        Assert.Equal(" \r\n   xada/l;fslffp{salfas;f", c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n   xada/l;fslf大大大fp{salfas;f{{ 大打发打发发发", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslf大大大fp{salfas;f", c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n   xada/l;fslf大大大fp{salfas;f{ 大打发打发发发", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslf大大大fp{salfas;f{ 大打发打发发发", c);
        Assert.Null(err);

        t = Any('{', true);
        Assert.True(t.TryParse(" \r\n   xada/l;fslffp{salfas;f{{", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslffp", c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n   xada/l;fslf大大大fpsalfas;f 大打发打发发发", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslf大大大fpsalfas;f 大打发打发发发", c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n   xada/l;fslf大大大fpsalfas;f 大打发打发发发", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslf大大大fpsalfas;f 大打发打发发发", c);
        Assert.Null(err);

        t = Any('{', true, true);
        Assert.True(t.TryParse(" \r\n   xada/l;fslffp{salfas;f{{", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslffp", c);
        Assert.Null(err);

        Assert.False(t.TryParse(" \r\n   xada/l;fslf大大大fpsalfas;f 大打发打发发发", out c, out err));
        Assert.Null(c.ToString());
        Assert.NotNull(err);
    }

    [Fact]
    public void BetweenText()
    {
        var t = Between(Text("{{"), Any("}}", false, true), Text("}}"));
        Assert.True(t.TryParse(" \r\n   {{ \r\n   sdda\r\ndad}} dada\r\n", out var c, out var err));
        Assert.Equal(" \r\n   sdda\r\ndad", c);
        Assert.Null(err);
    }
}