using System.Numerics;

namespace Lmzzz.Chars;

public static partial class Parsers
{
    public static Deferred<T> Deferred<T>() => new();

    public static Parser<T> InogreSeparator<T>(Parser<T> parser) => new InogreSeparator<T>(parser);

    public static Parser<T> Eof<T>(this Parser<T> parser) => new Eof<T>(parser);

    public static Parser<T> Between<A, T, B>(Parser<A> before, Parser<T> parser, Parser<B> after) => new Between<A, T, B>(before, parser, after);

    public static Parser<char> Char(char c) => InogreSeparator<char>(new CharLiteral(c));

    public static Parser<string> Text(string text, bool ordinalIgnoreCase = false) => InogreSeparator<string>(new TextLiteral(text, ordinalIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));

    public static Parser<TextSpan> Any(char end, bool ordinalIgnoreCase = false, bool mustHasEnd = false, char? escape = null) => Any(end.ToString(), ordinalIgnoreCase, mustHasEnd, escape);

    public static Parser<TextSpan> Any(string end, bool ordinalIgnoreCase = false, bool mustHasEnd = false, char? escape = null) => new AnyLiteral(end, ordinalIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal, mustHasEnd, escape);

    public static Parser<TextSpan> AnyBefore<T>(Parser<T> parser, bool canBeEmpty = false, bool failOnEof = false, bool consumeDelimiter = false) => new TextBefore<T>(parser, canBeEmpty, failOnEof, consumeDelimiter);

    public static Parser<T> ZeroOrOne<T>(Parser<T> parser, T defaultValue) => new ZeroOrOne<T>(parser, defaultValue);

    public static Parser<T> ZeroOrOne<T>(Parser<T> parser) where T : notnull => new ZeroOrOne<T>(parser, default!);

    public static Parser<IReadOnlyList<T>> ZeroOrMany<T>(Parser<T> parser) => new ZeroOrMany<T>(parser);

    public static Parser<IReadOnlyList<T>> OneOrMany<T>(Parser<T> parser) => new OneOrMany<T>(parser);

    public static Parser<IReadOnlyList<T>> Separated<U, T>(Parser<U> separator, Parser<T> parser) => new Separated<U, T>(separator, parser);

    public static Parser<TextSpan> String(char quotes = '"', char escape = '\\') => Between(Char(quotes), Any(quotes, mustHasEnd: true, escape: escape), Char(quotes));

    #region And

    public static Sequence<T1, T2> And<T1, T2>(this Parser<T1> parser, Parser<T2> and) => new(parser, and);

    public static Sequence<T1, T2, T3> And<T1, T2, T3>(this Sequence<T1, T2> parser, Parser<T3> and) => new(parser, and);

    #endregion And

    #region Or

    public static Parser<T> Or<T>(this Parser<T> parser, Parser<T> or)
    {
        if (parser is OneOf<T> oneOf)
        {
            return new OneOf<T>([.. oneOf.Parsers, or]);
        }
        else
        {
            return new OneOf<T>([parser, or]);
        }
    }

    public static Parser<T> OneOf<T>(params Parser<T>[] parsers) => new OneOf<T>(parsers);

    #endregion Or

    #region Number

    public static Parser<T> Number<T>(NumberOptions numberOptions = NumberOptions.Number, char decimalSeparator = '.', char groupSeparator = ',') where T : INumber<T>
        => InogreSeparator(new NumberLiteral<T>(numberOptions, decimalSeparator, groupSeparator));

    public static Parser<int> Int(NumberOptions numberOptions = NumberOptions.Integer) => Number<int>(numberOptions);

    public static Parser<long> Long(NumberOptions numberOptions = NumberOptions.Integer) => Number<long>(numberOptions);

    public static Parser<float> Float(NumberOptions numberOptions = NumberOptions.Float) => Number<float>(numberOptions);

    public static Parser<double> Double(NumberOptions numberOptions = NumberOptions.Float) => Number<double>(numberOptions);

    public static Parser<decimal> Decimal(NumberOptions numberOptions = NumberOptions.Float) => Number<decimal>(numberOptions);

    #endregion Number
}