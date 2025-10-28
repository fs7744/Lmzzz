using Lmzzz.Chars.Fluent;
using Lmzzz.JsonPath.Statements;
using static Lmzzz.Chars.Fluent.Parsers;

namespace Lmzzz.JsonPath;

public class JsonPathParser
{
    public static Parser<IStatement> JsonPathQuery = RootIdentifier.And(Segments).Then<IStatement>(static x => new RootNode() { Child = x.Item2 });
    public static Parser<IStatement> Segments = ZeroOrMany(S.And(Segment)).Then<IStatement>(static x => throw new NotImplementedException());

    public static Parser<char> B = Char(new char[]
    { (char)0x20, // Space
      (char)0x09, //Horizontal tab
      (char)0x0A, // Line feed or New line
      (char)0x0D // Carriage return
    });

    public static Parser<IReadOnlyList<char>> S = ZeroOrMany(B);
    public static Parser<char> RootIdentifier = Char('$');
    public static Parser<IStatement> Selector = NameSelector.Then<IStatement>(static x => throw new NotImplementedException()).Or(WildcardSelector.Then<IStatement>(static x => throw new NotImplementedException())).Or(SliceSelector).Or(IndexSelector.Then<IStatement>(static x => throw new NotImplementedException())).Or(FilterSelector);
    public static Parser<TextSpan> NameSelector = StringLiteral;
    public static Parser<TextSpan> StringLiteral = Between(DoubleQuoted, Unescaped, DoubleQuoted).Or(Between(SingleQuoted, Unescaped, SingleQuoted));
    public static Parser<char> DoubleQuoted = Char('"');
    public static Parser<char> SingleQuoted = Char('\'');
    public static Parser<TextSpan> Unescaped = Any("\\\r\n", mustHasEnd: true, escape: '\\');
    public static Parser<char> WildcardSelector = Char('*');
    public static Parser<decimal> IndexSelector = Int;
    public static Parser<decimal> Int = Decimal(NumberOptions.Integer);
    public static Parser<IStatement> SliceSelector = Optional(Start.And(S)).And(Char(':')).And(S).And(Optional(End.And(S))).And(Optional(Char(':').And(Optional(S.And(Step))))).Then<IStatement>(static x => throw new NotImplementedException());
    public static Parser<decimal> Start = Int;
    public static Parser<decimal> End = Int;
    public static Parser<decimal> Step = Int;

    public static Parser<IStatement> FilterSelector = Char('?').And(S).And(LogicalExpr).Then<IStatement>(static x => new UnaryOperaterStatement()
    {
        Operator = x.Item1.ToString(),
        Statement = x.Item3
    });

    public static Parser<IStatement> LogicalExpr = LogicalOrExpr;
    public static Parser<IStatement> LogicalOrExpr = LogicalAndExpr.And(ZeroOrMany(S.And(Text("||")).And(S).And(LogicalAndExpr))).Then<IStatement>(static x => throw new NotImplementedException());
    public static Parser<IStatement> LogicalAndExpr = BasicExpr.And(ZeroOrMany(S.And(Text("&&")).And(S).And(BasicExpr))).Then<IStatement>(static x => throw new NotImplementedException());
    public static Parser<IStatement> BasicExpr = ParenExpr.Or(ComparisonExpr).Or(TestExpr);

    public static Parser<IStatement> ParenExpr = Optional(LogicalNotOp.And(S)).And(Char('(')).And(S).And(LogicalExpr).And(S).And(Char(')'))
        .Then<IStatement>(static x => new UnaryOperaterStatement()
        {
            Operator = x.Item1.Item1.ToString(),
            Statement = x.Item4
        });

    public static Parser<char> LogicalNotOp = Char('!');
    public static Parser<IStatement> TestExpr = Optional(LogicalNotOp.And(S)).And(FilterQuery.Or(FunctionExpr)).Then<IStatement>(static x => new UnaryOperaterStatement() { Operator = x.Item1.Item1.ToString(), Statement = x.Item2 });
    public static Parser<IStatement> FilterQuery = RelQuery.Or(JsonPathQuery);
    public static Parser<IStatement> RelQuery = CurrentNodeIdentifier.And(Segments).Then<IStatement>(static x => new CurrentNode() { Child = x.Item2 });
    public static Parser<char> CurrentNodeIdentifier = Char('@');
    public static Parser<IStatement> ComparisonExpr = Comparable.And(S).And(ComparisonOp).And(S).And(Comparable).Then<IStatement>(static x => new OperatorStatement() { Left = x.Item1, Operator = x.Item3, Right = x.Item5 });
    public static Parser<IStatement> Literal = Num.Or(StringLiteral.Then<IStatement>(static x => new StringValue(x.Span.ToString()))).Or(True).Or(False).Or(Null);
    public static Parser<IStatement> Comparable = Literal.Or(SingularQuery).Or(FunctionExpr);
    public static Parser<string> ComparisonOp = Text("==").Or(Text("!=")).Or(Text("<=")).Or(Text(">=")).Or(Text("<")).Or(Text(">"));
    public static Parser<IStatement> SingularQuery = RelSingularQuery.Or(AbsSingularQuery);
    public static Parser<IStatement> RelSingularQuery = CurrentNodeIdentifier.And(SingularQuerySegments).Then<IStatement>(static x => throw new NotImplementedException());
    public static Parser<IStatement> AbsSingularQuery = RootIdentifier.And(SingularQuerySegments).Then<IStatement>(static x => throw new NotImplementedException());
    public static Parser<IStatement> SingularQuerySegments = ZeroOrMany(S.And(NameSegment.Or(IndexSegment))).Then<IStatement>(static x => throw new NotImplementedException());
    public static Parser<IStatement> NameSegment = Char('[').And(NameSelector).And(Char(']')).Then<IStatement>(static x => throw new NotImplementedException()).Or(Char('.').And(MemberNameShorthand).Then<IStatement>(static x => throw new NotImplementedException()));
    public static Parser<IStatement> IndexSegment = Char('[').And(IndexSelector).And(Char(']')).Then<IStatement>(static x => throw new NotImplementedException());
    public static Parser<IStatement> Num = Decimal(NumberOptions.Float).Then<IStatement>(static x => new NumberValue(x));
    public static Parser<IStatement> True = Text("true").Then<IStatement>(static x => BoolValue.True);
    public static Parser<IStatement> False = Text("false").Then<IStatement>(static x => BoolValue.False);
    public static Parser<IStatement> Null = Text("null").Then<IStatement>(static x => NullValue.Value);
    public static Parser<string> FunctionName = FunctionNameFirst.And(ZeroOrMany(FunctionNameChar)).Then<string>(static x => throw new NotImplementedException());
    public static Parser<char> FunctionNameFirst = LCALPHA;
    public static Parser<char> FunctionNameChar = FunctionNameFirst.Or(Char('_')).Or(DIGIT);
    public static Parser<char> LCALPHA = Char('a', 'z');
    public static Parser<IStatement> FunctionExpr = FunctionName.And(Char('(')).And(S).And(Optional(FunctionArgument.And(ZeroOrMany(S.And(Char(',')).And(S).And(FunctionArgument))))).And(S).And(Char(')')).Then<IStatement>(static x => throw new NotImplementedException());
    public static Parser<IStatement> FunctionArgument = Literal.Or(FilterQuery).Or(LogicalExpr).Or(FunctionExpr);
    public static Parser<IStatement> Segment = ChildSegment.Or(DescendantSegment);
    public static Parser<IStatement> ChildSegment = BracketedSelection.Or(Char('.').And(WildcardSelector.Then<IStatement>(static x => throw new NotImplementedException()).Or(MemberNameShorthand)).Then<IStatement>(static x => throw new NotImplementedException())).Then<IStatement>(static x => throw new NotImplementedException());
    public static Parser<IStatement> BracketedSelection = Char('[').And(S).And(Selector).And(ZeroOrMany(S.And(Char(',')).And(Selector))).And(S).And(Char(']')).Then<IStatement>(static x => throw new NotImplementedException());
    public static Parser<IStatement> MemberNameShorthand = NameFirst.And(ZeroOrMany(NameChar)).Then<IStatement>(static x => new Member { Name = x.Item1 + new string(x.Item2.ToArray()) });
    public static Parser<char> NameFirst = ALPHA.Or(Char('_')).Or(Char((char)0x80, (char)0xD7FF)).Or(Char((char)0xE000, (char)0xFFFF));
    public static Parser<char> NameChar = NameFirst.Or(DIGIT);
    public static Parser<char> DIGIT = Char('0', '9');
    public static Parser<char> ALPHA = Char((char)0x41, (char)0x5A).Or(Char((char)0x61, (char)0x7A));
    public static Parser<IStatement> DescendantSegment = Char('.').And(Char('.')).And(BracketedSelection.Then<IStatement>(static x => throw new NotImplementedException()).Or(WildcardSelector.Then<IStatement>(static x => throw new NotImplementedException())).Or(MemberNameShorthand)).Then<IStatement>(static x => throw new NotImplementedException());
}