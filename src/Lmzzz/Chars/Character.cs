using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Lmzzz.Chars.Fluent;

public static partial class Character
{
    public const char DefaultDecimalSeparator = '.';
    public const char DefaultDecimalGroupSeparator = ',';
    public const char NullChar = '\0';

    public const string DecimalDigits = "0123456789";
    public const string Alpha = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string AlphaNumeric = Alpha + DecimalDigits;
    public const string DefaultIdentifierStart = "$_" + Alpha;
    public const string DefaultIdentifierPart = "$_" + AlphaNumeric;
    public const string HexDigits = "0123456789abcdefABCDEF";
    public const string WhiteSpacesAscii = " \t\f\xa0";
    public const string WhiteSpacesNonAscii = "\x1680\x180E\x2000\x2001\x2002\x2003\x2004\x2005\x2006\x2007\x2008\x2009\x200a\x202f\x205f\x3000\xfeff";

    public const string NewLines = "\n\r\v";

    public static readonly SearchValues<char> SVDecimalDigits = SearchValues.Create(DecimalDigits);
    public static readonly SearchValues<char> SVHexDigits = SearchValues.Create(HexDigits);
    public static readonly SearchValues<char> SVIdentifierStart = SearchValues.Create(DefaultIdentifierStart);
    public static readonly SearchValues<char> SVIdentifierPart = SearchValues.Create(DefaultIdentifierPart);
    public static readonly SearchValues<char> SVWhiteSpacesAscii = SearchValues.Create(WhiteSpacesAscii);
    public static readonly SearchValues<char> SVWhiteSpacesNonAscii = SearchValues.Create(WhiteSpacesNonAscii);
    public static readonly SearchValues<char> SVWhiteSpaces = SearchValues.Create(WhiteSpacesAscii + WhiteSpacesNonAscii);
    public static readonly SearchValues<char> SVNewLines = SearchValues.Create(NewLines);
    public static readonly SearchValues<char> SVWhiteSpaceOrNewLines = SearchValues.Create(WhiteSpacesAscii + WhiteSpacesNonAscii + NewLines);
    public static readonly SearchValues<char> SVWhiteSpaceOrNewLinesAscii = SearchValues.Create(WhiteSpacesAscii + NewLines);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhiteSpaceOrNewLine(char ch)
    {
        return SVWhiteSpaceOrNewLinesAscii.Contains(ch);
    }

    public static NumberStyles ToNumberStyles(this NumberOptions numberOptions)
    {
        var numberStyles = NumberStyles.None;

        if (numberOptions.HasFlag(NumberOptions.AllowLeadingSign))
        {
            numberStyles |= NumberStyles.AllowLeadingSign;
        }

        if (numberOptions.HasFlag(NumberOptions.AllowDecimalSeparator))
        {
            numberStyles |= NumberStyles.AllowDecimalPoint;
        }

        if (numberOptions.HasFlag(NumberOptions.AllowGroupSeparators))
        {
            numberStyles |= NumberStyles.AllowThousands;
        }

        if (numberOptions.HasFlag(NumberOptions.AllowExponent))
        {
            numberStyles |= NumberStyles.AllowExponent;
        }

        return numberStyles;
    }
}