using System.Globalization;
using System.Numerics;
using System.Reflection;
using Lmzzz.Chars.Fluent;

namespace Lmzzz.Chars;

public sealed class NumberLiteral<T> : Parser<T>
    where T : INumber<T>
{
    private const char DefaultDecimalSeparator = '.';
    private const char DefaultGroupSeparator = ',';

    private static readonly MethodInfo _tryParseMethodInfo = typeof(T).GetMethod(nameof(INumber<T>.TryParse), [typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider), typeof(T).MakeByRefType()])!;

    private readonly char _decimalSeparator;
    private readonly char _groupSeparator;
    private readonly NumberStyles _numberStyles;
    private readonly CultureInfo _culture = CultureInfo.InvariantCulture;
    private readonly bool _allowLeadingSignPlus;
    private readonly bool _allowLeadingSignMinus;
    private readonly bool _allowDecimalSeparator;
    private readonly bool _allowGroupSeparator;
    private readonly bool _allowExponent;

    public NumberLiteral(NumberOptions numberOptions = NumberOptions.Number, char decimalSeparator = DefaultDecimalSeparator, char groupSeparator = DefaultGroupSeparator)
    {
        _decimalSeparator = decimalSeparator;
        _groupSeparator = groupSeparator;
        _numberStyles = numberOptions.ToNumberStyles();

        if (decimalSeparator != Character.DefaultDecimalSeparator ||
            groupSeparator != Character.DefaultDecimalGroupSeparator)
        {
            _culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            _culture.NumberFormat.NumberDecimalSeparator = decimalSeparator.ToString();
            _culture.NumberFormat.NumberGroupSeparator = groupSeparator.ToString();
        }

        _allowLeadingSignMinus = (numberOptions & NumberOptions.AllowLeadingSignMinus) != 0;
        _allowLeadingSignPlus = (numberOptions & NumberOptions.AllowLeadingSignPlus) != 0;
        _allowDecimalSeparator = (numberOptions & NumberOptions.AllowDecimalSeparator) != 0;
        _allowGroupSeparator = (numberOptions & NumberOptions.AllowGroupSeparators) != 0;
        _allowExponent = (numberOptions & NumberOptions.AllowExponent) != 0;
    }

    public override bool Parse(CharParseContext context, ref ParseResult<T> result)
    {
        context.EnterParser(this);

        var reset = context.Cursor.Position;
        var start = reset.Offset;

        if (context.Cursor.ReadDecimal(_allowLeadingSignPlus, _allowLeadingSignMinus, _allowDecimalSeparator, _allowGroupSeparator, _allowExponent, out var number, _decimalSeparator, _groupSeparator))
        {
            var end = context.Cursor.Offset;

            if (T.TryParse(number, _numberStyles, _culture, out var value))
            {
                result.Set(start, end, value);

                context.ExitParser(this);
                return true;
            }
        }

        context.Cursor.Reset(reset);

        context.ExitParser(this);
        return false;
    }
}