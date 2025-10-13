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

        Assert.False(t.TryParse(" \r\n  SeleCt 1", out c, out err));
        Assert.Null(c);
        Assert.Null(err);
    }

    [Fact]
    public void StringTest()
    {
        var t = String().Eof();

        Assert.True(t.TryParse("\"1\\\"2\"", out var c, out var err));
        Assert.Equal("1\\\"2", c);
        Assert.Null(err);

        Assert.True(t.TryParse("\"12\"", out c, out err));
        Assert.Equal("12", c);
        Assert.Null(err);

        Assert.False(t.TryParse("\"", out c, out err));
        Assert.Null(c.ToString());
        Assert.Null(err);

        Assert.True(t.TryParse("\"\"", out c, out err));
        Assert.Equal("", c);
        Assert.Null(err);

        t = String('\'').Eof();

        Assert.True(t.TryParse("'1\\'2'", out c, out err));
        Assert.Equal("1\\'2", c);
        Assert.Null(err);

        Assert.True(t.TryParse("'12'", out c, out err));
        Assert.Equal("12", c);
        Assert.Null(err);

        Assert.False(t.TryParse("'", out c, out err));
        Assert.Null(c.ToString());
        Assert.Null(err);

        Assert.True(t.TryParse("''", out c, out err));
        Assert.Equal("", c);
        Assert.Null(err);
    }

    [Fact]
    public void SeparatedTest()
    {
        var t = Separated(Char(','), Text("select", true)).Eof();
        Assert.True(t.TryParse("select", out var c, out var err));
        Assert.Single(c);
        Assert.Equal("select", c[0]);
        Assert.Null(err);

        Assert.True(t.TryParse("select,SElect", out c, out err));
        Assert.Equal(2, c.Count);
        Assert.Equal("select", c[0]);
        Assert.Equal("select", c[1]);
        Assert.Null(err);

        Assert.False(t.TryParse("se", out c, out err));
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
        Assert.Null(err);

        Assert.True(t.TryParse("{", out c, out err));
        Assert.Equal("", c.ToString());
        Assert.Null(err);
    }

    [Fact]
    public void AnyBeforeTest()
    {
        var t = AnyBefore(Text("{{"), true, consumeDelimiter: true);
        Assert.True(t.TryParse(" \r\n   xada/l;fslffp{salfas;f{{", out var c, out var err));
        Assert.Equal(" \r\n   xada/l;fslffp{salfas;f", c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n   xada/l;fslf大大大fp{salfas;f{{ 大打发打发发发", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslf大大大fp{salfas;f", c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n   xada/l;fslf大大大fp{salfas;f{ 大打发打发发发", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslf大大大fp{salfas;f{ 大打发打发发发", c);
        Assert.Null(err);

        t = AnyBefore(Char('{'), true, consumeDelimiter: true);
        Assert.True(t.TryParse(" \r\n   xada/l;fslffp{salfas;f{{", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslffp", c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n   xada/l;fslf大大大fpsalfas;f 大打发打发发发", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslf大大大fpsalfas;f 大打发打发发发", c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n   xada/l;fslf大大大fpsalfas;f 大打发打发发发", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslf大大大fpsalfas;f 大打发打发发发", c);
        Assert.Null(err);

        t = AnyBefore(Char('{'), true, true);
        Assert.True(t.TryParse(" \r\n   xada/l;fslffp{salfas;f{{", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslffp", c);
        Assert.Null(err);

        Assert.False(t.TryParse(" \r\n   xada/l;fslf大大大fpsalfas;f 大打发打发发发", out c, out err));
        Assert.Null(c.ToString());
        Assert.Null(err);
    }

    [Fact]
    public void BetweenText()
    {
        var t = Between(Text("{{"), Any("}}", false, true), Text("}}"));
        Assert.True(t.TryParse(" \r\n   {{ \r\n   sdda\r\ndad}} dada\r\n", out var c, out var err));
        Assert.Equal(" \r\n   sdda\r\ndad", c);
        Assert.Null(err);
    }

    [Fact]
    public void ZeroOrOneText()
    {
        var t = ZeroOrOne(Between(Text("{{"), Any("}}", false, true), Text("}}")));
        Assert.True(t.TryParse(" \r\n   {{ \r\n   sdda\r\ndad}} dada\r\n", out var c, out var err));
        Assert.Equal(" \r\n   sdda\r\ndad", c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n   ", out c, out err));
        Assert.Null(c.ToString());
        Assert.Null(err);
    }

    [Fact]
    public void ZeroOrManyText()
    {
        var t = ZeroOrMany(Between(Text("{{"), Any("}}", false, true), Text("}}")));
        Assert.True(t.TryParse(" \r\n   {{ \r\n   sdda\r\ndad}} dada\r\n", out var c, out var err));
        Assert.Single(c);
        Assert.Equal(" \r\n   sdda\r\ndad", c[0]);
        Assert.Null(err);

        Assert.True(t.TryParse("{{ \r\n   sdda\r\ndad}} {{ 2}}", out c, out err));
        Assert.Equal(2, c.Count);
        Assert.Equal(" \r\n   sdda\r\ndad", c[0]);
        Assert.Equal(" 2", c[1]);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n   ", out c, out err));
        Assert.Empty(c);
        Assert.Null(err);
    }

    [Fact]
    public void OneOrManyText()
    {
        var t = OneOrMany(Between(Text("{{"), Any("}}", false, true), Text("}}")));
        Assert.True(t.TryParse(" \r\n   {{ \r\n   sdda\r\ndad}} dada\r\n", out var c, out var err));
        Assert.Single(c);
        Assert.Equal(" \r\n   sdda\r\ndad", c[0]);
        Assert.Null(err);

        Assert.True(t.TryParse("{{ \r\n   sdda\r\ndad}} {{ 2}}", out c, out err));
        Assert.Equal(2, c.Count);
        Assert.Equal(" \r\n   sdda\r\ndad", c[0]);
        Assert.Equal(" 2", c[1]);
        Assert.Null(err);

        Assert.False(t.TryParse(" \r\n   ", out c, out err));
        Assert.Null(c);
        Assert.Null(err);
    }
}