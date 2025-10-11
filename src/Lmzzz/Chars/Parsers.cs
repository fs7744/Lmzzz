namespace Lmzzz.Chars;

public static class Parsers
{
    public static TermBuilder Terms => new();

    public static Parser<T> InogreSeparator<T>(Parser<T> parser) => new InogreSeparator<T>(parser);

    public static Parser<T> Eof<T>(this Parser<T> parser) => new Eof<T>(parser);
}

public class TermBuilder
{
    public Parser<char> Char(char c) => Parsers.InogreSeparator<char>(new CharLiteral(c));

    public Parser<string> Text(string text, bool ordinalIgnoreCase = false) => Parsers.InogreSeparator<string>(new TextLiteral(text, ordinalIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));

    public Parser<TextSpan> Any(char end, bool ordinalIgnoreCase = false, bool mustHasEnd = false) => Any(end.ToString(), ordinalIgnoreCase, mustHasEnd);

    public Parser<TextSpan> Any(string end, bool ordinalIgnoreCase = false, bool mustHasEnd = false) => new AnyLiteral(end, ordinalIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal, mustHasEnd);
}