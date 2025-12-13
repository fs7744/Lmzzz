using System.Buffers;
using System.Numerics;

namespace Lmzzz.Chars.Fluent;

public static partial class Parsers
{
    public static Deferred<T> Deferred<T>(string name = null) => new() { Name = name };

    public static Parser<T> Name<T>(this Parser<T> parser, string name)
    {
        parser.Name = name;
        return parser;
    }

    public static Parser<T> IgnoreSeparator<T>(Parser<T> parser) => new IgnoreSeparator<T>(parser);

    public static Parser<T> Eof<T>(this Parser<T> parser, bool ignoreSeparator = true) => new Eof<T>(parser, ignoreSeparator);

    public static Parser<T> Else<T>(this Parser<T> parser, T value) => new Else<T>(parser, value);

    public static Parser<T> ElseError<T>(this Parser<T> parser, string message) => new ElseError<T>(parser, message);

    public static Parser<T> Between<A, T, B>(Parser<A> before, Parser<T> parser, Parser<B> after) => new Between<A, T, B>(before, parser, after);

    public static Parser<char> Char(char c) => new CharLiteral(c);

    public static Parser<char> Char(char start, char end)
    {
        return new CharLiterals(string.Join("", Enumerable.Range((int)start, (int)end - (int)start).Select(i => (char)i)));
    }

    public static Parser<char> Char(char[] chars)
    {
        return new CharLiterals(string.Join("", chars));
    }

    public static Parser<char> Char(string chars)
    {
        return new CharLiterals(chars);
    }

    public static Parser<Nothing> IgnoreChar(char c) => new IgnoreCharLiteral(c.ToString());

    public static Parser<Nothing> IgnoreChar(char start, char end) => new IgnoreCharLiteral(string.Join("", Enumerable.Range((int)start, (int)end - (int)start).Select(i => (char)i)));

    public static Parser<Nothing> IgnoreChar(char[] chars) => new IgnoreCharLiteral(string.Join("", chars));

    public static Parser<Nothing> IgnoreChar(string chars) => new IgnoreCharLiteral(chars);

    public static Parser<string> Text(string text, bool ordinalIgnoreCase = false) => new TextLiteral(text, ordinalIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

    public static Parser<TextSpan> AnyExclude(string exclude) => new ExcludeLiteral(exclude);

    public static Parser<TextSpan> Any(char end, bool mustHasEnd = false, bool canEmpty = true, char? escape = null) => Any(end.ToString(), mustHasEnd, canEmpty, escape);

    public static Parser<TextSpan> Any(string end, bool mustHasEnd = false, bool canEmpty = true, char? escape = null) => new AnyLiteral(end, mustHasEnd, canEmpty, escape);

    public static Parser<TextSpan> AnyBeforeEnd(string end, bool mustHasEnd = false, bool canEmpty = true, char? escape = null) => new AnyBeforeEndLiteral(end, mustHasEnd, canEmpty, escape);

    public static Parser<TextSpan> AnyBefore<T>(Parser<T> parser, bool canBeEmpty = false, bool failOnEof = false, bool consumeDelimiter = false) => new TextBefore<T>(parser, canBeEmpty, failOnEof, consumeDelimiter);

    public static Parser<T> ZeroOrOne<T>(Parser<T> parser, T defaultValue) => new ZeroOrOne<T>(parser, defaultValue);

    public static Parser<T> ZeroOrOne<T>(Parser<T> parser) where T : notnull => new ZeroOrOne<T>(parser, default!);

    public static Parser<T> Optional<T>(Parser<T> parser, T defaultValue) => ZeroOrOne(parser, defaultValue);

    public static Parser<T> Optional<T>(Parser<T> parser) where T : notnull => ZeroOrOne(parser);

    public static Parser<IReadOnlyList<T>> ZeroOrMany<T>(Parser<T> parser) => new ZeroOrMany<T>(parser);

    public static Parser<Nothing> IgnoreZeroOrMany<T>(Parser<T> parser) => new IgnoreZeroOrMany<T>(parser);

    public static Parser<IReadOnlyList<T>> OneOrMany<T>(Parser<T> parser) => new OneOrMany<T>(parser);

    public static Parser<IReadOnlyList<T>> Separated<U, T>(Parser<U> separator, Parser<T> parser) => new Separated<U, T>(separator, parser);

    public static Parser<TextSpan> String(char quotes = '"', char escape = '\\') => Between(Char(quotes), Any(quotes, mustHasEnd: true, escape: escape), Char(quotes));

    public static Parser<TextSpan> Identifier(SearchValues<char> identifierStart, SearchValues<char> identifierPart) => new IdentifierLiteral(identifierStart, identifierPart);

    public static Parser<TextSpan> Identifier(ReadOnlySpan<char> identifierStart, ReadOnlySpan<char> identifierPart) => new IdentifierLiteral(SearchValues.Create(identifierStart), SearchValues.Create(identifierPart));

    #region And

    public static Sequence<T1, T2> And<T1, T2>(this Parser<T1> parser, Parser<T2> and) => new(parser, and);

    public static Sequence<T1, T2, T3> And<T1, T2, T3>(this Sequence<T1, T2> parser, Parser<T3> and) => new(parser, and);

    public static Sequence<T1, T2, T3, T4> And<T1, T2, T3, T4>(this Sequence<T1, T2, T3> parser, Parser<T4> and) => new(parser, and);

    public static Sequence<T1, T2, T3, T4, T5> And<T1, T2, T3, T4, T5>(this Sequence<T1, T2, T3, T4> parser, Parser<T5> and) => new(parser, and);

    public static Sequence<T1, T2, T3, T4, T5, T6> And<T1, T2, T3, T4, T5, T6>(this Sequence<T1, T2, T3, T4, T5> parser, Parser<T6> and) => new(parser, and);

    public static Sequence<T1, T2, T3, T4, T5, T6, T7> And<T1, T2, T3, T4, T5, T6, T7>(this Sequence<T1, T2, T3, T4, T5, T6> parser, Parser<T7> and) => new(parser, and);

    #endregion And

    #region Or

    public static Parser<char> Or(this Parser<char> parser, Parser<char> or)
    {
        string cs = null;
        if (parser is CharLiteral c)
        {
            cs = c.Value;
        }
        else if (parser is CharLiterals cc)
        {
            cs = cc.Value;
        }
        else if (parser is IgnoreSeparator<char> isc)
        {
            if (isc.Parser is CharLiteral c1)
                cs = c1.Value;
            else if (isc.Parser is CharLiterals c2)
                cs = c2.Value;
        }

        if (cs != null)
        {
            string csc = null;
            if (or is CharLiteral scc)
            {
                csc = scc.Value;
            }
            else if (or is CharLiterals sccc)
            {
                cs = sccc.Value;
            }
            else if (or is IgnoreSeparator<char> iscc)
            {
                if (iscc.Parser is CharLiteral cc1)
                    csc = cc1.Value;
                else if (iscc.Parser is CharLiterals cc2)
                    csc = cc2.Value;
            }

            if (csc != null)
            {
                return new CharLiterals(cs + csc);
            }
        }

        return Or<char>(parser, or);
    }

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
        => new NumberLiteral<T>(numberOptions, decimalSeparator, groupSeparator);

    public static Parser<int> Int(NumberOptions numberOptions = NumberOptions.Integer) => Number<int>(numberOptions);

    public static Parser<long> Long(NumberOptions numberOptions = NumberOptions.Integer) => Number<long>(numberOptions);

    public static Parser<float> Float(NumberOptions numberOptions = NumberOptions.Float) => Number<float>(numberOptions);

    public static Parser<double> Double(NumberOptions numberOptions = NumberOptions.Float) => Number<double>(numberOptions);

    public static Parser<decimal> Decimal(NumberOptions numberOptions = NumberOptions.Float) => Number<decimal>(numberOptions);

    #endregion Number

    public static Parser<T> LeftAssociative<T, TInput>(this Parser<T> parser, params (Parser<TInput> op, Func<T, T, T> factory)[] list)
    {
        var choices = list.Select(l => new Then<TInput, Func<T, T, T>>(l.op, c => l.factory)).ToArray();

        return parser.And(ZeroOrMany(new OneOf<Func<T, T, T>>(choices).And(parser)))
            .Then(static x =>
            {
                // multiplicative
                var result = x.Item1;

                // (("-" | "+") multiplicative ) *
                var xx = x.Item2;
                for (int i = 0; i < xx.Count; i++)
                {
                    var op = xx[i];
                    result = op.Item1(result, op.Item2);
                }
                //foreach (var op in x.Item2)
                //{
                //    result = op.Item1(result, op.Item2);
                //}

                return result;
            });
    }
}