namespace Lmzzz.Chars;

public static class Parsers
{
    public static TermBuilder Terms => new();

    public static Parser<T> InogreSeparator<T>(Parser<T> parser) => new InogreSeparator<T>(parser);
}

public class TermBuilder
{
    public Parser<char> Char(char c) => Parsers.InogreSeparator<char>(new CharLiteral(c));

    public Parser<string> Text(string text, bool ordinalIgnoreCase = false) => Parsers.InogreSeparator<string>(new TextLiteral(text, ordinalIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
}