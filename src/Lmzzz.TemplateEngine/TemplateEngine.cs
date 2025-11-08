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

    public static readonly Parser<IStatement> NumberValue = IgnoreSeparator(Decimal()).Then<IStatement>(static s => new DecimalValueStatement(s)).Name(nameof(NumberValue));
    public static readonly Parser<IStatement> Field = IgnoreSeparator(Separated(Char('.'), Identifier(Character.SVIdentifierStart, Character.SVIdentifierPart))).Then<IStatement>(static s => new FieldStatement(s)).Name(nameof(Field));

    public static readonly Parser<char> ParenOpen = IgnoreSeparator(Char('(')).Name(nameof(ParenOpen));
    public static readonly Parser<char> ParenClose = IgnoreSeparator(Char(')')).Name(nameof(ParenClose));
    public static readonly Deferred<IStatement> FunctionExpr = Deferred<IStatement>(nameof(FunctionExpr));
    public static readonly Parser<IStatement> AnyValue = NullValue.Or(BoolValue).Or(NumberValue).Or(IgnoreSeparator(FunctionExpr)).Or(Field).Name(nameof(AnyValue));

    public static readonly Parser<IStatement> OP = AnyValue
        .And(IgnoreSeparator(Text("==")).Or(IgnoreSeparator(Text("!="))).Or(IgnoreSeparator(Text(">"))).Or(IgnoreSeparator(Text(">="))).Or(IgnoreSeparator(Text("<"))).Or(IgnoreSeparator(Text("<="))))
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

    static TemplateEngineParser()
    {
        FunctionExpr.Parser = Identifier(Character.SVIdentifierStart, Character.SVIdentifierPart).And(ParenOpen)
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