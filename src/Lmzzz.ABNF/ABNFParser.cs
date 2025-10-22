using Lmzzz.Chars;
using Lmzzz.Chars.Fluent;
using static Lmzzz.Chars.Fluent.Parsers;

namespace Lmzzz.ABNF;

public interface IStatement
{ }

public class ABNFParser
{
    //; A-Z / a-z
    public static Parser<char> ALPHA = Char((char)0x41, (char)0x5A).Or(Char((char)0x61, (char)0x7A));

    public static Parser<char> BIT = Char((char)0).Or(Char((char)1));

    //; any 7-bit US-ASCII character,
    //; excluding NUL
    public static Parser<char> CHAR = Char((char)0x01, (char)0x7F);

    //; carriage return
    public static Parser<char> CR = Char((char)0x0D);

    //; Internet standard newline
    public static Sequence<char, char> CRLF = CR.And(LF);

    //; controls
    public static Parser<char> CTL = Char((char)0x00, (char)0x1F).Or(Char((char)0x7F));

    //; 0-9
    public static Parser<char> DIGIT = Char((char)0x30, (char)0x39);

    //; " (Double Quote)
    public static Parser<char> DQUOTE = Char((char)0x22);

    public static Parser<char> HEXDIG = DIGIT.Or(Char('A')).Or(Char('B')).Or(Char('C')).Or(Char('D')).Or(Char('E')).Or(Char('F'));

    //; horizontal tab
    public static Parser<char> HTAB = Char((char)0x09);

    //; linefeed
    public static Parser<char> LF = Char((char)0x0A);

    public static Parser<IReadOnlyList<string>> LWSP = ZeroOrMany(WSP.Then<string>(static c => c.ToString()).Or(CRLF.And(WSP).Then<string>(static (x) => string.Concat(x.Item1, x.Item2, x.Item3))));

    //; 8 bits of data
    public static Parser<char> OCTET = Char((char)0x00, (char)0xFF);

    public static Parser<char> SP = Char((char)0x20);

    //; visible (printing) characters
    public static Parser<char> VCHAR = Char((char)0x21, (char)0x7E);

    //; white space
    public static Parser<char> WSP = SP.Or(HTAB);

    public static Sequence<char, IReadOnlyList<char>, IReadOnlyList<(char, IReadOnlyList<char>)>> dec_val = Char('d').And(OneOrMany(DIGIT)).And(Optional(OneOrMany(Char('.').And(OneOrMany(DIGIT))).Or(OneOrMany(Char('-').And(OneOrMany(DIGIT))))));

    public static Sequence<char, IReadOnlyList<char>, IReadOnlyList<(char, IReadOnlyList<char>)>> hex_val = Char('x').And(OneOrMany(HEXDIG)).And(Optional(OneOrMany(Char('.').And(OneOrMany(HEXDIG))).Or(OneOrMany(Char('-').And(OneOrMany(HEXDIG))))));

    public static Parser<IReadOnlyList<char>> prose_val = Between(Char('<'), ZeroOrMany(Char((char)0x20, (char)0x3D).Or(Char((char)0x3F, (char)0x7E))), Char('>'));
}