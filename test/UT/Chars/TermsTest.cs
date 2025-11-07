namespace UT.Chars;

using Lmzzz;
using Lmzzz.Chars;
using Lmzzz.Chars.Fluent;
using static Lmzzz.Chars.Fluent.Parsers;

public class TermsTest
{
    [Fact]
    public void CharTest()
    {
        var t = IgnoreSeparator(Char('{')).Eof();
        Assert.True(t.TryParse("{", out var c, out var err));
        Assert.Equal('{', c);
        Assert.Null(err);

        Assert.False(t.TryParse("}", out c, out err));
        Assert.Equal(Character.NullChar, c);
        Assert.NotNull(err);

        Assert.True(t.TryParse(" \r\n  {", out c, out err));
        Assert.Equal('{', c);
        Assert.Null(err);
    }

    [Fact]
    public void TextTest()
    {
        var t = IgnoreSeparator(Text("select", true)).Eof();
        Assert.True(t.TryParse("select", out var c, out var err));
        Assert.Equal("select", c);
        Assert.Null(err);

        Assert.False(t.TryParse("se", out c, out err));
        Assert.Null(c);
        Assert.NotNull(err);

        Assert.True(t.TryParse(" \r\n  SeleCt", out c, out err));
        Assert.Equal("select", c);
        Assert.Null(err);

        Assert.False(t.TryParse(" \r\n  SeleCt 1", out c, out err));
        Assert.Null(c);
        Assert.NotNull(err);
    }

    [Fact]
    public void StringTest()
    {
        var t = String().Eof();

        Assert.True(t.TryParse("\"1\\\"2\"", out var c, out var err));
        Assert.Equal("1\"2", c);
        Assert.Null(err);

        Assert.True(t.TryParse("\"12\"", out c, out err));
        Assert.Equal("12", c);
        Assert.Null(err);

        Assert.False(t.TryParse("\"", out c, out err));
        Assert.Null(c.ToString());
        Assert.NotNull(err);

        Assert.True(t.TryParse("\"\"", out c, out err));
        Assert.Equal("", c);
        Assert.Null(err);

        t = String('\'').Eof();

        Assert.True(t.TryParse("'1\\'2'", out c, out err));
        Assert.Equal("1'2", c);
        Assert.Null(err);

        Assert.True(t.TryParse("'12'", out c, out err));
        Assert.Equal("12", c);
        Assert.Null(err);

        Assert.False(t.TryParse("'", out c, out err));
        Assert.Null(c.ToString());
        Assert.NotNull(err);

        Assert.True(t.TryParse("''", out c, out err));
        Assert.Equal("", c);
        Assert.Null(err);

        Assert.True(t.TryParse("'1\\\\\\'2'", out c, out err));
        Assert.Equal("1\\'2", c);
        Assert.Null(err);

        Assert.True(t.TryParse("'1\\\\\\'2dasdadadad\\\\sdsdadad\\\\adas'", out c, out err));
        Assert.Equal("1\\'2dasdadadad\\sdsdadad\\adas", c);
        Assert.Null(err);

        Assert.False(t.TryParse("'1\\\\\\'2dasdadadad\\sdsdadad\\\\adas'", out c, out err));
        Assert.NotNull(err);

        Assert.False(t.TryParse("'1\\\\\\'2dasdadadad\\", out c, out err));
        Assert.NotNull(err);
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
        Assert.NotNull(err);
    }

    [Fact]
    public void AnyTest()
    {
        var t = Any("{", true);
        Assert.True(t.TryParse(" \r\n   xada/l;fslffp{salfas;f{{", out var c, out var err));
        Assert.Equal(" \r\n   xada/l;fslffp", c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n   xada/l;fslf大大大fp{salfas;f{{ 大打发打发发发", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslf大大大fp", c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n   xada/l;fslf大大大fp{salfas;f{ 大打发打发发发", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslf大大大fp", c);
        Assert.Null(err);

        t = Any('{', false);
        Assert.True(t.TryParse(" \r\n   xada/l;fslffp{salfas;f{{", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslffp", c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n   xada/l;fslf大大大fpsalfas;f 大打发打发发发", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslf大大大fpsalfas;f 大打发打发发发", c);
        Assert.Null(err);

        Assert.True(t.TryParse(" \r\n   xada/l;fslf大大大fpsalfas;f 大打发打发发发", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslf大大大fpsalfas;f 大打发打发发发", c);
        Assert.Null(err);

        t = Any('{', true);
        Assert.True(t.TryParse(" \r\n   xada/l;fslffp{salfas;f{{", out c, out err));
        Assert.Equal(" \r\n   xada/l;fslffp", c);
        Assert.Null(err);

        Assert.False(t.TryParse(" \r\n   xada/l;fslf大大大fpsalfas;f 大打发打发发发", out c, out err));
        Assert.Null(c.ToString());
        Assert.NotNull(err);

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
        Assert.NotNull(err);
    }

    [Fact]
    public void BetweenText()
    {
        var t = Between(IgnoreSeparator(Text("{{")), Any("}}", true), IgnoreSeparator(Text("}}")));
        Assert.True(t.TryParse(" \r\n   {{ \r\n   sdda\r\ndad}} dada\r\n", out var c, out var err));
        Assert.Equal(" \r\n   sdda\r\ndad", c);
        Assert.Null(err);
    }

    [Fact]
    public void ZeroOrOneText()
    {
        var t = ZeroOrOne(Between(IgnoreSeparator(Text("{{")), Any("}}", true), IgnoreSeparator(Text("}}"))));
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
        var t = ZeroOrMany(Between(IgnoreSeparator(Text("{{")), Any("}}", true), IgnoreSeparator(Text("}}"))));
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
        var t = OneOrMany(Between(IgnoreSeparator(Text("{{")), Any("}}", true), IgnoreSeparator(Text("}}"))));
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
        Assert.NotNull(err);
    }

    [Fact]
    public void IdentifierText()
    {
        var field = Identifier(Character.SVIdentifierStart, Character.SVIdentifierPart).Then<string>(static s => s.ToString());
        var dot = Char('.');
        var fp = Separated(dot, field).Then(static s => s).Eof();
        Assert.True(fp.TryParse("f.f1", out var c, out var err));
        Assert.Equal(["f", "f1"], c);

        Assert.True(fp.TryParse("f.f1.y", out c, out err));
        Assert.Equal(["f", "f1", "y"], c);

        Assert.True(fp.TryParse("f", out c, out err));
        Assert.Equal(["f"], c);
    }

    [Fact]
    public void IgnoreZeroOrManyText()
    {
        var f = IgnoreZeroOrMany(Char(" \r\n"));
        Assert.True(f.TryParse(" ", out var c, out var err));
        Assert.Equal(Nothing.Value, c);

        Assert.True(f.TryParse("f.f1.y", out c, out err));
        Assert.Equal(Nothing.Value, c);
    }

    [Fact]
    public void IgnoreCharText()
    {
        var f = IgnoreChar(" \r\n");
        var c = new ParseResult<Nothing>();
        Assert.True(f.Parse(new CharParseContext(new StringCursor(" \r\n23d \r\n")), ref c));
        Assert.Equal(Nothing.Value, c.Value);
        Assert.Equal(0, c.Start);
        Assert.Equal(2, c.End);

        Assert.True(f.Parse(new CharParseContext(new StringCursor("f.f1.y")), ref c));
        Assert.Equal(Nothing.Value, c.Value);
        Assert.Equal(0, c.Start);
        Assert.Equal(0, c.End);
    }

    //[Fact]
    //public void Test()
    //{
    //    //Assert.Equal(char.MaxValue, (char)0x41);
    //    //Assert.Equal(1, 0x41);
    //    Assert.Equal(1, 0b101);
    //}
}