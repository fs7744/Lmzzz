using System.Buffers;
using System.Runtime.CompilerServices;

namespace Lmzzz.Chars;

public static partial class Character
{
    public const char NullChar = '\0';

    internal const string DecimalDigits = "0123456789";
    internal const string Alpha = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    internal const string AlphaNumeric = Alpha + DecimalDigits;
    internal const string DefaultIdentifierStart = "$_" + Alpha;
    internal const string DefaultIdentifierPart = "$_" + AlphaNumeric;
    internal const string HexDigits = "0123456789abcdefABCDEF";
    internal const string WhiteSpacesAscii = " \t\f\xa0";
    internal const string WhiteSpacesNonAscii = "\x1680\x180E\x2000\x2001\x2002\x2003\x2004\x2005\x2006\x2007\x2008\x2009\x200a\x202f\x205f\x3000\xfeff";

    internal const string NewLines = "\n\r\v";

    internal static readonly SearchValues<char> SVDecimalDigits = SearchValues.Create(DecimalDigits);
    internal static readonly SearchValues<char> SVHexDigits = SearchValues.Create(HexDigits);
    internal static readonly SearchValues<char> SVIdentifierStart = SearchValues.Create(DefaultIdentifierStart);
    internal static readonly SearchValues<char> SVIdentifierPart = SearchValues.Create(DefaultIdentifierPart);
    internal static readonly SearchValues<char> SVWhiteSpacesAscii = SearchValues.Create(WhiteSpacesAscii);
    internal static readonly SearchValues<char> SVWhiteSpacesNonAscii = SearchValues.Create(WhiteSpacesNonAscii);
    internal static readonly SearchValues<char> SVWhiteSpaces = SearchValues.Create(WhiteSpacesAscii + WhiteSpacesNonAscii);
    internal static readonly SearchValues<char> SVNewLines = SearchValues.Create(NewLines);
    internal static readonly SearchValues<char> SVWhiteSpaceOrNewLines = SearchValues.Create(WhiteSpacesAscii + WhiteSpacesNonAscii + NewLines);
    internal static readonly SearchValues<char> SVWhiteSpaceOrNewLinesAscii = SearchValues.Create(WhiteSpacesAscii + NewLines);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhiteSpaceOrNewLine(char ch)
    {
        return SVWhiteSpaceOrNewLinesAscii.Contains(ch);
    }
}