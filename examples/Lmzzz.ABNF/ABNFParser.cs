using Lmzzz.Chars;
using Lmzzz.Chars.Fluent;
using static Lmzzz.Chars.Fluent.Parsers;

namespace Lmzzz.ABNF;

public interface IStatement
{ }

public partial class AbstractABNFParser
{
    //; A-Z / a-z
    public static Parser<IStatement> ALPHA = Convert_ALPHA(Char((char)0x41, (char)0x5A).Or(Char((char)0x61, (char)0x7A)));

    public static Parser<IStatement> BIT = Convert_BIT(Char((char)0).Or(Char((char)1)));

    //; any 7-bit US-ASCII character,
    //; excluding NUL
    public static Parser<IStatement> CHAR = Convert_CHAR(Char((char)0x01, (char)0x7F));

    //; carriage return
    public static Parser<IStatement> CR = Convert_CR(Char((char)0x0D));

    //; Internet standard newline
    public static Parser<IStatement> CRLF = Convert_CRLF(CR.And(LF));

    //; controls
    public static Parser<IStatement> CTL = Convert_CTL(Char((char)0x00, (char)0x1F).Or(Char((char)0x7F)));

    //; 0-9
    public static Parser<IStatement> DIGIT = Convert_DIGIT(Char((char)0x30, (char)0x39));

    //; " (Double Quote)
    public static Parser<IStatement> DQUOTE = Convert_DQUOTE(Char((char)0x22));

    public static Parser<IStatement> HEXDIG = DIGIT.Or(Convert_HEXDIG_1(Char('A').Or(Char('B')).Or(Char('C')).Or(Char('D')).Or(Char('E')).Or(Char('F'))));

    //; horizontal tab
    public static Parser<IStatement> HTAB = Convert_HTAB(Char((char)0x09));

    //; linefeed
    public static Parser<IStatement> LF = Convert_LF(Char((char)0x0A));

    public static Parser<IStatement> LWSP = Convert_LWSP(ZeroOrMany(WSP.Or(Convert_LWSP_1(CRLF.And(WSP)))));

    //; 8 bits of data
    public static Parser<IStatement> OCTET = Convert_OCTET(Char((char)0x00, (char)0xFF));

    public static Parser<IStatement> SP = Convert_SP(Char((char)0x20));

    //; visible (printing) characters
    public static Parser<IStatement> VCHAR = Convert_VCHAR(Char((char)0x21, (char)0x7E));

    //; white space
    public static Parser<IStatement> WSP = Convert_WSP(SP.Or(HTAB));

    public static Parser<IStatement> c_wsp = Convert_c_wsp(WSP.Or(Convert_c_wsp_1(c_nl.And(WSP))));

    public static Parser<IStatement> c_nl = Convert_c_nl(comment.Or(CRLF));

    public static Parser<IStatement> rulelist = Convert_rulelist(OneOrMany(rule.Or(Convert_rulelist_1(ZeroOrMany(c_wsp).And(c_nl)))));

    public static Parser<IStatement> rule = Convert_rule(rulename.And(defined_as).And(elements).And(c_nl));

    public static Parser<IStatement> rulename = Convert_rulename(ALPHA.And(ZeroOrMany(ALPHA.Or(DIGIT).Or(Convert_rulename_1(Char('-'))))));

    public static Parser<IStatement> defined_as = Convert_defined_as(ZeroOrMany(c_wsp).And(Text("=").Or(Text("=/"))).And(ZeroOrMany(c_wsp)));

    public static Parser<IStatement> elements = Convert_elements(alternation.And(ZeroOrMany(c_wsp)));

    public static Parser<IStatement> alternation = Convert_alternation(concatenation.And(ZeroOrMany(ZeroOrMany(c_wsp).And(Char('/').And(ZeroOrMany(c_wsp)).And(concatenation)))));

    public static Parser<IStatement> concatenation = Convert_concatenation(repetition.And(ZeroOrMany(OneOrMany(c_wsp).And(repetition))));

    public static Parser<IStatement> repetition = Convert_repetition(Optional(repeat).And(element));

    public static Parser<IStatement> comment = Convert_comment(Between(Char(';'), ZeroOrMany(WSP.Or(VCHAR)), CRLF));

    public static Parser<IStatement> repeat = Convert_repeat(OneOrMany(DIGIT).Or(Convert_repeat_1(ZeroOrMany(DIGIT).And(Char('*').And(ZeroOrMany(DIGIT))))));

    public static Parser<IStatement> element = rulename.Or(group).Or(option).Or(char_val).Or(num_val).Or(prose_val);

    public static Parser<IStatement> group = Convert_group(Char('(').And(ZeroOrMany(c_wsp)).And(alternation).And(ZeroOrMany(c_wsp)).And(Char(')')));

    public static Parser<IStatement> option = Convert_option(Char('[').And(ZeroOrMany(c_wsp)).And(alternation).And(ZeroOrMany(c_wsp)).And(Char(']')));

    public static Parser<IStatement> char_val = Convert_char_val(Between(DQUOTE, ZeroOrMany(Char((char)0x20, (char)0x21).Or(Char((char)0x23, (char)0x7E))), DQUOTE));

    public static Parser<IStatement> num_val = Convert_num_val(Char('%').And(bin_val.Or(dec_val).Or(hex_val)));

    public static Parser<IStatement> bin_val = Convert_bin_val(Char('b').And(OneOrMany(BIT)).And(Optional(OneOrMany(Char('.').And(OneOrMany(BIT))).Or(OneOrMany(Char('-').And(OneOrMany(BIT)))))));

    public static Parser<IStatement> dec_val = Convert_dec_val(Char('d').And(OneOrMany(DIGIT)).And(Optional(OneOrMany(Char('.').And(OneOrMany(DIGIT))).Or(OneOrMany(Char('-').And(OneOrMany(DIGIT)))))));

    public static Parser<IStatement> hex_val = Convert_hex_val(Char('x').And(OneOrMany(HEXDIG)).And(Optional(OneOrMany(Char('.').And(OneOrMany(HEXDIG))).Or(OneOrMany(Char('-').And(OneOrMany(HEXDIG)))))));

    public static Parser<IStatement> prose_val = Convert_prose_val(Between(Char('<'), ZeroOrMany(Char((char)0x20, (char)0x3D).Or(Char((char)0x3F, (char)0x7E))), Char('>')));
}

public partial class AbstractABNFParser
{
    private static Parser<IStatement> Convert_ALPHA(Parser<char> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_BIT(Parser<char> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_CHAR(Parser<char> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_CR(Parser<char> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_CRLF(Sequence<IStatement, IStatement> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_CTL(Parser<char> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_DIGIT(Parser<char> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_DQUOTE(Parser<char> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_HEXDIG_1(Parser<char> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_HTAB(Parser<char> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_LF(Parser<char> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_LWSP(Parser<IReadOnlyList<IStatement>> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_LWSP_1(Sequence<IStatement, IStatement> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_OCTET(Parser<char> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_SP(Parser<char> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_VCHAR(Parser<char> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_WSP(Parser<IStatement> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_c_wsp(Parser<IStatement> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_c_wsp_1(Sequence<IStatement, IStatement> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_c_nl(Parser<IStatement> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_comment(Parser<IReadOnlyList<IStatement>> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_char_val(Parser<IReadOnlyList<char>> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_num_val(Sequence<char, IStatement> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_bin_val(Sequence<char, IReadOnlyList<IStatement>, IReadOnlyList<(char, IReadOnlyList<IStatement>)>> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_dec_val(Sequence<char, IReadOnlyList<IStatement>, IReadOnlyList<(char, IReadOnlyList<IStatement>)>> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_hex_val(Sequence<char, IReadOnlyList<IStatement>, IReadOnlyList<(char, IReadOnlyList<IStatement>)>> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_prose_val(Parser<IReadOnlyList<char>> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_repeat(Parser<IReadOnlyList<IStatement>> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IReadOnlyList<IStatement>> Convert_repeat_1(Sequence<IReadOnlyList<IStatement>, (char, IReadOnlyList<IStatement>)> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_rulename(Sequence<IStatement, IReadOnlyList<IStatement>> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_rulename_1(Parser<char> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_defined_as(Sequence<IReadOnlyList<IStatement>, string, IReadOnlyList<IStatement>> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_repetition(Sequence<IStatement, IStatement> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_group(Sequence<char, IReadOnlyList<IStatement>, IStatement, IReadOnlyList<IStatement>, char> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_option(Sequence<char, IReadOnlyList<IStatement>, IStatement, IReadOnlyList<IStatement>, char> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_rule(Sequence<IStatement, IStatement, IStatement, IStatement> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_elements(Sequence<IStatement, IReadOnlyList<IStatement>> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_alternation(Sequence<IStatement, IReadOnlyList<(IReadOnlyList<IStatement>, (char, IReadOnlyList<IStatement>, IStatement))>> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_concatenation(Sequence<IStatement, IReadOnlyList<(IReadOnlyList<IStatement>, IStatement)>> sequence)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_rulelist(Parser<IReadOnlyList<IStatement>> parser)
    {
        throw new NotImplementedException();
    }

    private static Parser<IStatement> Convert_rulelist_1(Sequence<IReadOnlyList<IStatement>, IStatement> sequence)
    {
        throw new NotImplementedException();
    }
}