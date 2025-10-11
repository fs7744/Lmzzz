namespace Lmzzz.Chars;

public static class Parsers
{
    public static Parser<T> InogreSeparator<T>(Parser<T> parser) => new InogreSeparator<T>(parser);

    public static Parser<T> Eof<T>(this Parser<T> parser) => new Eof<T>(parser);

    public static Parser<T> Between<A, T, B>(Parser<A> before, Parser<T> parser, Parser<B> after) => new Between<A, T, B>(before, parser, after);

    public static Parser<char> Char(char c) => Parsers.InogreSeparator<char>(new CharLiteral(c));

    public static Parser<string> Text(string text, bool ordinalIgnoreCase = false) => Parsers.InogreSeparator<string>(new TextLiteral(text, ordinalIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));

    public static Parser<TextSpan> Any(char end, bool ordinalIgnoreCase = false, bool mustHasEnd = false) => Any(end.ToString(), ordinalIgnoreCase, mustHasEnd);

    public static Parser<TextSpan> Any(string end, bool ordinalIgnoreCase = false, bool mustHasEnd = false) => new AnyLiteral(end, ordinalIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal, mustHasEnd);
}