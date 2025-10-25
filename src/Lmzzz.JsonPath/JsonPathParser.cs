using Lmzzz.Chars;
using Lmzzz.Chars.Fluent;
using static Lmzzz.Chars.Fluent.Parsers;

namespace Lmzzz.JsonPath;

public class JsonPathParser
{
    public static Parser<IStatement> JsonPathQuery = RootIdentifier.And(Segments);
    public static Parser<IStatement> Segments = ZeroOrMany(S.And(segment));

    public static Parser<char> B = Char(new char[]
    { (char)0x20, // Space
      (char)0x09, //Horizontal tab
      (char)0x0A, // Line feed or New line
      (char)0x0D // Carriage return
    });

    public static Parser<IReadOnlyList<char>> S = ZeroOrMany(B);
    public static Parser<char> RootIdentifier = Char('$');
    public static Parser<IStatement> Selector = NameSelector.Or(WildcardSelector).Or(SliceSelector).Or(IndexSelector).Or(FilterSelector);
    public static Parser<TextSpan> NameSelector = StringLiteral;
    public static Parser<TextSpan> StringLiteral = Between(DoubleQuoted, Unescaped, DoubleQuoted).Or(Between(SingleQuoted, Unescaped, SingleQuoted));
    public static Parser<char> DoubleQuoted = Char('"');
    public static Parser<char> SingleQuoted = Char('\'');
    public static Parser<TextSpan> Unescaped = Any("\\\r\n", mustHasEnd: true, escape: '\\');
    public static Parser<char> WildcardSelector = Char('*');
    public static Parser<decimal> IndexSelector = Int;
    public static Parser<decimal> Int = Decimal(NumberOptions.Integer);
    public static Sequence<(decimal, IReadOnlyList<char>), char, IReadOnlyList<char>, (decimal, IReadOnlyList<char>), (char, (IReadOnlyList<char>, decimal))> SliceSelector = Optional(Start.And(S)).And(Char(':')).And(S).And(Optional(End.And(S))).And(Optional(Char(':').And(Optional(S.And(Step)))));
    public static Parser<decimal> Start = Int;
    public static Parser<decimal> End = Int;
    public static Parser<decimal> Step = Int;
    public static Parser<char> FilterSelector = Char('?').And(S).And(LogicalExpr);
    public static Parser<char> LogicalExpr = LogicalOrExpr;
    public static Parser<char> LogicalOrExpr = LogicalAndExpr.And(ZeroOrMany(S.And(Text("||")).And(S).And(LogicalAndExpr)));
    public static Parser<char> LogicalAndExpr = BasicExpr.And(ZeroOrMany(S.And(Text("&&")).And(S).And(BasicExpr)));
    public static Parser<char> BasicExpr = ParenExpr.Or(ComparisonExpr).Or(TestExpr);
    public static Parser<char> ParenExpr = Optional(LogicalNotOp.And(S)).And(Char('(')).And(S).And(LogicalExpr).And(S).And(Char(')'));
    public static Parser<char> LogicalNotOp = Char('!');
    public static Parser<char> TestExpr = Optional(LogicalNotOp.And(S)).And(FilterQuery.Or(FunctionExpr));
    public static Parser<char> FilterQuery = RelQuery.Or(JsonPathQuery);
    public static Parser<char> RelQuery = CurrentNodeIdentifier.And(Segments);
    public static Parser<char> CurrentNodeIdentifier = Char('@');
    public static Parser<char> ComparisonExpr = Comparable.And(S).And(ComparisonOp).And(S).And(Comparable);
    public static Parser<char> Literal = Num.Or(StringLiteral).Or(True).Or(False).Or(Null);
    public static Parser<char> Comparable = Literal.Or(SingularQuery).Or(FunctionExpr);
    public static Parser<string> ComparisonOp = Text("==").Or(Text("!=")).Or(Text("<=")).Or(Text(">=")).Or(Text("<")).Or(Text(">"));
    public static Parser<char> SingularQuery = RelSingularQuery.Or(AbsSingularQuery);
    public static Parser<char> RelSingularQuery = CurrentNodeIdentifier.And(SingularQuerySegments);
    public static Parser<char> AbsSingularQuery = RootIdentifier.And(SingularQuerySegments);
    public static Parser<char> SingularQuerySegments = ZeroOrMany(S.And(NameSegment.Or(IndexSegment)));
    public static Parser<char> NameSegment = Char('[').And(NameSelector).And(Char(']')).Or(Char('.').And(MemberNameShorthand));
    public static Sequence<char, decimal, char> IndexSegment = Char('[').And(IndexSelector).And(Char(']'));
    public static Parser<decimal> Num = Decimal(NumberOptions.Float);
    public static Parser<string> True = Text("true");
    public static Parser<string> False = Text("false");
    public static Parser<string> Null = Text("null");
    public static Parser<string> FunctionName = FunctionNameFirst.And(ZeroOrMany(FunctionNameChar));
    public static Parser<char> FunctionNameFirst = LCALPHA;
    public static Parser<char> FunctionNameChar = FunctionNameFirst.Or(Char('_')).Or(DIGIT);
    public static Parser<char> LCALPHA = Char('a', 'z');
    public static Parser<char> FunctionExpr = FunctionName.And(Char('(')).And(S).And(Optional(FunctionArgument.And(ZeroOrMany(S.And(Char(',')).And(S).And(FunctionArgument))))).And(S).And(Char(')'));
    public static Parser<char> FunctionArgument = Literal.Or(FilterQuery).Or(LogicalExpr).Or(FunctionExpr);
    public static Parser<char> Segment = ChildSegment.Or(DescendantSegment);
    public static Parser<char> ChildSegment = BracketedSelection.Or(Char('.').And(WildcardSelector.Or(MemberNameShorthand)));
    public static Parser<char> BracketedSelection = Char('[').And(S).And(Selector).And(ZeroOrMany(S.And(Char(',')).And(Selector))).And(S).And(Char(']'));
    public static Parser<char> MemberNameShorthand = NameFirst.And(ZeroOrMany(NameChar));
    public static Parser<char> NameFirst = ALPHA.Or(Char('_')).Or(Char((char)0x80, (char)0xD7FF)).Or(Char((char)0xE000, (char)0xFFFF));
    public static Parser<char> NameChar = NameFirst.Or(DIGIT);
    public static Parser<char> DIGIT = Char('0', '9');
    public static Parser<char> ALPHA = Char((char)0x41, (char)0x5A).Or(Char((char)0x61, (char)0x7A));
    public static Parser<char> DescendantSegment = Text("..").And(BracketedSelection.Or(WildcardSelector).Or(MemberNameShorthand));
}