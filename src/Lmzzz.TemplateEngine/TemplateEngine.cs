using Lmzzz.Chars;
using Lmzzz.Chars.Fluent;
using static Lmzzz.Chars.Fluent.Parsers;

namespace Lmzzz.Template.Inner;

public class TemplateEngineParser
{
    public static readonly Parser<IStatement> ConditionParser;
    public static readonly Parser<IStatement> NullValue = IgnoreSeparator(Text("null")).Then<IStatement>(static s => NullValueStatement.Value).Name(nameof(NullValue));

    public static readonly Parser<IStatement> BoolValue = IgnoreSeparator(Text("true", true)).Then<IStatement>(static s => BoolValueStatement.True)
            .Or(IgnoreSeparator(Text("false", true)).Then<IStatement>(static s => BoolValueStatement.False)).Name(nameof(BoolValue));

    public static readonly Parser<IStatement> StringValue = IgnoreSeparator(String('\'').Or(String())).Then<IStatement>(static s => new StringValueStatement(s)).Name(nameof(StringValue));
    public static readonly Parser<IStatement> NumberValue = IgnoreSeparator(Decimal()).Then<IStatement>(static s => new DecimalValueStatement(s)).Name(nameof(NumberValue));
    public static readonly Parser<IStatement> Field = IgnoreSeparator(Separated(Char('.'), Identifier(Character.SVIdentifierPart, Character.SVIdentifierPart))).Then<IStatement>(static s => new FieldStatement(s)).Name(nameof(Field));

    public static readonly Parser<char> ParenOpen = IgnoreSeparator(Char('(')).Name(nameof(ParenOpen));
    public static readonly Parser<char> ParenClose = IgnoreSeparator(Char(')')).Name(nameof(ParenClose));
    public static readonly Deferred<IStatement> FunctionExpr = Deferred<IStatement>(nameof(FunctionExpr));
    public static readonly Parser<IStatement> AnyValue = NullValue.Or(BoolValue).Or(StringValue).Or(IgnoreSeparator(FunctionExpr)).Or(NumberValue).Or(Field).Name(nameof(AnyValue));

    public static readonly Parser<IStatement> OP = AnyValue
        .And(IgnoreSeparator(Text("==")).Or(IgnoreSeparator(Text("!="))).Or(IgnoreSeparator(Text(">="))).Or(IgnoreSeparator(Text(">"))).Or(IgnoreSeparator(Text("<="))).Or(IgnoreSeparator(Text("<"))))
        .And(AnyValue)
        .Then<IStatement>(static s => StatementUtils.Create(s.Item2, s.Item1, s.Item3)).Name(nameof(OP));

    public static readonly Deferred<IStatement> Condition = Deferred<IStatement>(nameof(Condition));

    public static readonly Parser<IStatement> GroupExpression = Between(ParenOpen, Condition, ParenClose).Name(nameof(GroupExpression));
    public static readonly Parser<IStatement> NotExpression = Between(IgnoreSeparator(Text("!")).And(ParenOpen), Condition, ParenClose).Then<IStatement>(static s => StatementUtils.Create("!", s)).Name(nameof(NotExpression));

    public static readonly Parser<IStatement> ConditionValue = OP.Or(IgnoreSeparator(FunctionExpr)).Or(BoolValue).Name(nameof(ConditionValue));

    public static readonly Parser<IStatement> Primary = NotExpression.Or(GroupExpression).Or(ConditionValue).Name(nameof(Primary));

    public static readonly Parser<IStatement> Conditions = Primary.LeftAssociative(
               (IgnoreSeparator(Text("&&")), static (x, y) => StatementUtils.Create("&&", x, y)),
               (IgnoreSeparator(Text("and", true)), static (x, y) => StatementUtils.Create("&&", x, y)),
               (IgnoreSeparator(Text("||")), static (x, y) => StatementUtils.Create("||", x, y)),
               (IgnoreSeparator(Text("or", true)), static (x, y) => StatementUtils.Create("||", x, y))
           ).Name(nameof(Conditions));

    public static readonly Parser<char> Sign = IgnoreSeparator(Char('@'));

    public static readonly Parser<IStatement> ReplaceStr = Between(Char('@'), AnyValue, Sign).Then<IStatement>(static x => new ReplaceStatement(x));

    public static readonly Parser<IStatement> OriginStr = Any('@', escape: '\\').Then<IStatement>(static x => new OriginStrStatement(x.Span.ToString()));

    public static readonly Parser<IStatement> RStr = ZeroOrMany(ReplaceStr.Or(OriginStr)).Then<IStatement>(static x =>
    {
        if (x == null || x.Count == 0) return null;
        var c = x[0];
        var list = new List<IStatement>() { c };
        for (var i = 1; i < x.Count; i++)
        {
            var n = x[i];
            if (c is OriginStrStatement o && n is OriginStrStatement no)
            {
                o.Text += no.Text;
                continue;
            }
            list.Add(n);
            c = n;
        }
        return list.Count == 1 ? list.First() : new ArrayStatement(list.ToArray());
    });

    public static readonly Parser<IStatement> If = Sign.And(IgnoreSeparator(Text("if", true))).And(ParenOpen).And(Conditions).And(ParenClose).And(Sign)
        .And(RStr)
        .And(ZeroOrMany(Sign.And(IgnoreSeparator(Text("elseif", true))).And(ParenOpen).And(Conditions).And(ParenClose).And(Sign).And(RStr)))
        .And(Optional(Sign.And(IgnoreSeparator(Text("else", true))).And(Sign).And(RStr)))
        .And(Sign.And(IgnoreSeparator(Text("endif", true))).And(Sign))
        .Then<IStatement>(static x => new IfStatement()
        {
            If = new IfConditionStatement(x.Item1.Item4, x.Item1.Item7),
            ElseIfs = x.Item2 == null || x.Item2.Count == 0 ? null : x.Item2.Select(static y => new IfConditionStatement(y.Item4, y.Item7)),
            Else = x.Item3.Item2 != null ? x.Item3.Item4 : null,
        })
        .Name(nameof(If));

    static TemplateEngineParser()

    {
        FunctionExpr.Parser = Identifier(Character.SVIdentifierPart, Character.SVIdentifierPart).And(ParenOpen)
            .And(Optional(AnyValue.And(ZeroOrMany(IgnoreSeparator(Char(',')).And(AnyValue))))).And(ParenClose).Then<IStatement>(static x =>
            {
                var args = new List<IStatement>();
                if (x.Item3.Item1 != null)
                {
                    args.Add(x.Item3.Item1);
                }
                if (x.Item3.Item2 != null)
                {
                    args.AddRange(x.Item3.Item2.Select(y => y.Item2));
                }
                var func = new FunctionStatement()
                {
                    Name = x.Item1.Span.ToString(),
                    Arguments = args.Count == 0 ? Array.Empty<IStatement>() : args.ToArray()
                };

                return func;
            });
        ConditionParser = Condition.Parser = Conditions;
        ConditionParser = ConditionParser.Eof();
    }
}