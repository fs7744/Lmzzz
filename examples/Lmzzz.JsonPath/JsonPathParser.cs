using Lmzzz.Chars;
using Lmzzz.Chars.Fluent;
using Lmzzz.JsonPath.Statements;
using static Lmzzz.Chars.Fluent.Parsers;

namespace Lmzzz.JsonPath;

public class JsonPathParser
{
    //public static Parser<char> B = Char(new char[]
    //{ (char)0x20, // Space
    //  (char)0x09, //Horizontal tab
    //  (char)0x0A, // Line feed or New line
    //  (char)0x0D // Carriage return
    //});

    public static readonly Parser<char> RootIdentifier = Char('$').Name(nameof(RootIdentifier));
    public static readonly Parser<int> Int = Int().Name(nameof(Int));
    public static readonly Parser<char> DoubleQuoted = Char('"').Name(nameof(DoubleQuoted));
    public static readonly Parser<char> SingleQuoted = Char('\'').Name(nameof(SingleQuoted));
    public static readonly Parser<IStatement> WildcardSelector = Char('*').Then<IStatement>(static x => new WildcardSelectorStatment()).Name(nameof(WildcardSelector));
    public static readonly Parser<IStatement> IndexSelector = Int.Then<IStatement>(static x => new IndexSelectorStatment() { Index = x }).Name(nameof(IndexSelector));
    public static readonly Parser<TextSpan> StringLiteral = Between(DoubleQuoted, ZeroOrOne(Any("\"", mustHasEnd: true, escape: '\\')), DoubleQuoted).Or(Between(SingleQuoted, ZeroOrOne(Any("'", mustHasEnd: true, escape: '\\')), SingleQuoted)).Name(nameof(StringLiteral));
    public static readonly Parser<IStatement> NameSelector = StringLiteral.Then<IStatement>(static x => new Member() { Name = x.Span.ToString() }).Name(nameof(NameSelector));
    public static readonly Parser<int> Start = Int;
    public static readonly Parser<int> End = Int;
    public static readonly Parser<int> Step = Int;

    public static readonly Parser<Nothing> S = IgnoreChar(new char[]
    { (char)0x20, // Space
      (char)0x09, //Horizontal tab
      (char)0x0A, // Line feed or New line
      (char)0x0D // Carriage return
    }).Name(nameof(S));

    public static readonly Parser<char> CurrentNodeIdentifier = Char('@').Name(nameof(CurrentNodeIdentifier));
    public static readonly Parser<char> LogicalNotOp = Char('!').Name(nameof(LogicalNotOp));
    public static readonly Parser<string> ComparisonOp = Text("==").Or(Text("!=")).Or(Text("<=")).Or(Text(">=")).Or(Text("<")).Or(Text(">")).Name(nameof(ComparisonOp));
    public static readonly Parser<IStatement> Num = Decimal(NumberOptions.Float).Then<IStatement>(static x => new NumberValue(x)).Name(nameof(Num));
    public static readonly Parser<IStatement> True = Text("true").Then<IStatement>(static x => BoolValue.True).Name(nameof(True));
    public static readonly Parser<IStatement> False = Text("false").Then<IStatement>(static x => BoolValue.False).Name(nameof(False));
    public static readonly Parser<IStatement> Null = Text("null").Then<IStatement>(static x => NullValue.Value).Name(nameof(Null));

    private const string name = "[]().,\" '\r\n@$!=<\\?&|*:";

    //public static Parser<char> LCALPHA = Char('a', 'z');
    //public static Parser<char> DIGIT = Char('0', '9');
    //public static Parser<char> ALPHA = Char((char)0x41, (char)0x5A).Or(Char((char)0x61, (char)0x7A));
    public static readonly Parser<IStatement> MemberNameShorthand = AnyExclude(name).Then<IStatement>(static x => new Member { Name = x.Span.ToString() }).Name(nameof(MemberNameShorthand));

    //public static Parser<char> NameFirst = ALPHA.Or(Char('_')).Or(Char((char)0x80, (char)0xD7FF)).Or(Char((char)0xE000, (char)0xFFFF));
    //public static Parser<char> NameChar = NameFirst.Or(DIGIT);
    //public static Parser<char> FunctionNameFirst = LCALPHA;

    //public static Parser<char> FunctionNameChar = FunctionNameFirst.Or(Char('_')).Or(DIGIT);
    //public static Parser<string> FunctionName = FunctionNameFirst.And(ZeroOrMany(FunctionNameChar)).Then<string>(static x => throw new NotImplementedException());
    public static readonly Parser<string> FunctionName = AnyExclude(name).Then<string>(static x => x.Span.ToString()).Name(nameof(FunctionName));

    public static readonly Parser<IStatement> SliceSelector = Optional<int?>(Start.And(S).Then<int?>(static x => x.Item1), null).And(Char(':')).And(S).And(Optional<int?>(End.And(S).Then<int?>(static x => x.Item1), null)).And(Optional<int?>(Char(':').And(Optional<int?>(S.And(Step).Then<int?>(static x => x.Item2), null)).Then<int?>(static x => x.Item2))).Then<IStatement>(static x => new SliceStatement() { Start = x.Item1, End = x.Item4, Step = x.Item5 })
        .Name(nameof(SliceSelector));

    public static readonly Deferred<IStatement> LogicalExpr = Deferred<IStatement>(nameof(LogicalExpr));

    public static readonly Parser<IStatement> FilterSelector = Char('?').And(S).And(LogicalExpr).Then<IStatement>(static x => new FilterSelectorStatement()
    {
        Statement = x.Item3
    }).Name(nameof(FilterSelector));

    public static readonly Parser<IStatement> Selector = NameSelector.Or(WildcardSelector).Or(SliceSelector).Or(IndexSelector).Or(FilterSelector).Name(nameof(Selector));

    public static readonly Parser<IStatement> ParenExpr = Optional(LogicalNotOp.And(S)).And(Char('(')).And(S).And(LogicalExpr).And(S).And(Char(')'))
        .Then<IStatement>(static x => new UnaryOperaterStatement()
        {
            Operator = x.Item1.Item1 == '!' ? "!" : "(",
            Statement = x.Item4
        }).Name(nameof(ParenExpr));

    public static readonly Deferred<IReadOnlyList<(Nothing, IStatement)>> Segments = Deferred<IReadOnlyList<(Nothing, IStatement)>>(nameof(Segments));

    public static readonly Deferred<IStatement> FunctionExpr = Deferred<IStatement>(nameof(FunctionExpr));
    public static readonly Deferred<IStatement> JsonPathQuery = Deferred<IStatement>(nameof(JsonPathQuery));
    public static readonly Parser<IStatement> RelQuery = CurrentNodeIdentifier.And(Segments).Then<IStatement>(static x => new CurrentNode() { Child = ConvertSegments(x.Item2) }).Name(nameof(RelQuery));
    public static readonly Parser<IStatement> Literal = Num.Or(StringLiteral.Then<IStatement>(static x => new StringValue(x.Span.ToString()))).Or(True).Or(False).Or(Null).Name(nameof(Literal));
    public static readonly Parser<IStatement> NameSegment = Char('[').And(NameSelector).And(Char(']')).Then<IStatement>(static x => x.Item2).Or(Char('.').And(MemberNameShorthand).Then<IStatement>(static x => x.Item2)).Name(nameof(NameSegment));
    public static readonly Parser<IStatement> IndexSegment = Char('[').And(IndexSelector).And(Char(']')).Then<IStatement>(static x => x.Item2).Name(nameof(IndexSegment));

    public static readonly Parser<IStatement> SingularQuerySegments = ZeroOrMany(S.And(NameSegment.Or(IndexSegment))).Then<IStatement>(ConvertSegments).Name(nameof(SingularQuerySegments));

    public static readonly Parser<IStatement> RelSingularQuery = CurrentNodeIdentifier.And(SingularQuerySegments).Then<IStatement>(static x => new CurrentNode() { Child = x.Item2 }).Name(nameof(RelSingularQuery));
    public static readonly Parser<IStatement> AbsSingularQuery = RootIdentifier.And(SingularQuerySegments).Then<IStatement>(static x => new RootNode() { Child = x.Item2 }).Name(nameof(AbsSingularQuery));
    public static readonly Parser<IStatement> SingularQuery = RelSingularQuery.Or(AbsSingularQuery).Name(nameof(SingularQuery));
    public static readonly Parser<IStatement> Comparable = Literal.Or(SingularQuery).Or(FunctionExpr).Name(nameof(Comparable));
    public static readonly Parser<IStatement> ComparisonExpr = Comparable.And(S).And(ComparisonOp).And(S).And(Comparable).Then<IStatement>(static x => new OperatorStatement() { Left = x.Item1, Operator = x.Item3, Right = x.Item5 }).Name(nameof(ComparisonExpr));
    public static readonly Parser<IStatement> FilterQuery = RelQuery.Or(JsonPathQuery).Name(nameof(FilterQuery));
    public static readonly Parser<IStatement> FunctionArgument = FilterQuery.Or(LogicalExpr).Or(FunctionExpr).Or(Literal).Name(nameof(FunctionArgument));
    public static readonly Parser<IStatement> TestExpr = Optional(LogicalNotOp.And(S)).And(FilterQuery.Or(FunctionExpr)).Then<IStatement>(static x => x.Item1.Item1 == '!' ? new UnaryOperaterStatement() { Operator = "!", Statement = x.Item2 } : x.Item2).Name(nameof(TestExpr));
    public static readonly Parser<IStatement> BasicExpr = ParenExpr.Or(ComparisonExpr).Or(TestExpr).Name(nameof(BasicExpr));

    public static readonly Parser<IStatement> LogicalAndExpr = BasicExpr.And(ZeroOrMany(S.And(Text("&&")).And(S).And(BasicExpr))).Then<IStatement>(static x =>
    {
        IStatement current = x.Item1;
        if (x.Item2 != null && x.Item2.Count > 0)
        {
            foreach (var item in x.Item2)
            {
                current = new AndStatement() { Left = current, Right = item.Item4 };
            }
        }
        return current;
    }).Name(nameof(LogicalAndExpr));

    public static readonly Parser<IStatement> LogicalOrExpr = LogicalAndExpr.And(ZeroOrMany(S.And(Text("||")).And(S).And(LogicalAndExpr))).Then<IStatement>(static x =>
    {
        IStatement current = x.Item1;
        if (x.Item2 != null && x.Item2.Count > 0)
        {
            foreach (var item in x.Item2)
            {
                current = new OrStatement() { Left = current, Right = item.Item4 };
            }
        }
        return current;
    }).Name(nameof(LogicalOrExpr));

    public static readonly Parser<IStatement> BracketedSelection = Char('[').And(S).And(Selector).And(ZeroOrMany(S.And(Char(',')).And(S).And(Selector))).And(S).And(Char(']'))
        .Then<IStatement>(static x =>
    {
        var list = new List<IStatement> { x.Item3 };
        if (x.Item4 != null)
            list.AddRange(x.Item4.Select(y => y.Item4));
        if (list.Count == 0)
            return null;
        return list.Count == 1 ? list[0] : new UnionSelectionStatement(list);
    }).Name(nameof(BracketedSelection));

    public static readonly Parser<IStatement> ChildSegment = BracketedSelection.Or(Char('.').And(WildcardSelector.Or(MemberNameShorthand)).Then<IStatement>(static x => x.Item2)).Name(nameof(ChildSegment));

    public static readonly Parser<IStatement> DescendantSegment = Char('.').And(Char('.')).And(BracketedSelection.Or(WildcardSelector).Or(MemberNameShorthand)).Then<IStatement>(static x => new WildcardSelectorStatment() { Child = x.Item3 }).Name(nameof(DescendantSegment));
    public static readonly Parser<IStatement> Segment = ChildSegment.Or(DescendantSegment).Name(nameof(Segment));

    public static readonly Parser<IStatement> Parser;

    static JsonPathParser()
    {
        LogicalExpr.Parser = LogicalOrExpr;
        Segments.Parser = ZeroOrMany(S.And(Segment));
        //MemberNameShorthand.Parser = NameFirst.And(ZeroOrMany(NameChar)).Then<IStatement>(static x => new Member { Name = x.Item1 + new string(x.Item2.ToArray()) });
        FunctionExpr.Parser = FunctionName.And(Char('(')).And(S).And(Optional(FunctionArgument.And(ZeroOrMany(S.And(Char(',')).And(S).And(FunctionArgument))))).And(S).And(Char(')')).Then<IStatement>(static x =>
        {
            var args = new List<IStatement>();
            if (x.Item4.Item1 != null)
            {
                args.Add(x.Item4.Item1);
            }
            if (x.Item4.Item2 != null)
            {
                args.AddRange(x.Item4.Item2.Select(y => y.Item4));
            }
            var func = new FunctionStatement()
            {
                Name = x.Item1,
                Arguments = args.Count == 0 ? Array.Empty<IStatement>() : args.ToArray()
            };

            return func;
        });
        JsonPathQuery.Parser = RootIdentifier.And(Segments).Then<IStatement>(static x => new RootNode() { Child = ConvertSegments(x.Item2) });
        Parser = JsonPathQuery.Eof().Name(nameof(Parser));
    }

    private static IStatement ConvertSegments(IReadOnlyList<(Nothing, IStatement)> x)
    {
        if (x == null || x.Count == 0)
        {
            return null;
        }
        else if (x.Count == 1)
            return x[0].Item2;
        else
        {
            var current = x.Last().Item2;
            for (int i = x.Count - 2; i >= 0; i--)
            {
                if (x[i].Item2 is IParentStatement p)
                {
                    var pp = p;
                    while (pp.Child != null)
                    {
                        var pc = p.Child as IParentStatement;
                        if (pc is null)
                            throw new NotSupportedException($"Cannot set child for statement of type {p.GetType().FullName}");
                        pp = pc;
                    }
                    pp.Child = current;
                    current = p;
                }
                else
                {
                    throw new NotSupportedException($"Cannot set child for statement of type {x[i].Item2.GetType().FullName}");
                }
            }
            return current;
        }
    }
}