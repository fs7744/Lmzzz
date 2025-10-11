namespace Lmzzz.Chars;

public static class Parsers
{
    public static Parser<T> InogreSeparator<T>(Parser<T> parser) => new InogreSeparator<T>(parser);

    public static Parser<T> Eof<T>(this Parser<T> parser) => new Eof<T>(parser);

    public static Parser<T> Between<A, T, B>(Parser<A> before, Parser<T> parser, Parser<B> after) => new Between<A, T, B>(before, parser, after);

    public static Parser<char> Char(char c) => InogreSeparator<char>(new CharLiteral(c));

    public static Parser<string> Text(string text, bool ordinalIgnoreCase = false) => InogreSeparator<string>(new TextLiteral(text, ordinalIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));

    public static Parser<TextSpan> Any(char end, bool ordinalIgnoreCase = false, bool mustHasEnd = false) => Any(end.ToString(), ordinalIgnoreCase, mustHasEnd);

    public static Parser<TextSpan> Any(string end, bool ordinalIgnoreCase = false, bool mustHasEnd = false) => new AnyLiteral(end, ordinalIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal, mustHasEnd);

    public static Parser<TextSpan> AnyBefore<T>(Parser<T> parser, bool canBeEmpty = false, bool failOnEof = false, bool consumeDelimiter = false) => new TextBefore<T>(parser, canBeEmpty, failOnEof, consumeDelimiter);

    public static Parser<T> ZeroOrOne<T>(Parser<T> parser, T defaultValue) => new ZeroOrOne<T>(parser, defaultValue);

    public static Parser<T> ZeroOrOne<T>(Parser<T> parser) where T : notnull => new ZeroOrOne<T>(parser, default!);

    public static Parser<IReadOnlyList<T>> ZeroOrMany<T>(Parser<T> parser) => new ZeroOrMany<T>(parser);

    public static Parser<IReadOnlyList<T>> OneOrMany<T>(Parser<T> parser) => new OneOrMany<T>(parser);
}