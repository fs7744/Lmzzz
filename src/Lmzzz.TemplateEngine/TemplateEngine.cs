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

    public static readonly Parser<IStatement> AnyValue = NullValue.Or(BoolValue).Or(NumberValue).Or(Field).Name(nameof(AnyValue));

    public static readonly Parser<IStatement> OP = AnyValue
        .And(IgnoreSeparator(Text("==")).Or(IgnoreSeparator(Text("!="))).Or(IgnoreSeparator(Text(">"))).Or(IgnoreSeparator(Text(">="))).Or(IgnoreSeparator(Text("<"))).Or(IgnoreSeparator(Text("<="))))
        .And(AnyValue)
        .Then<IStatement>(static s => new OperaterStatement() { Left = s.Item1, Operater = s.Item2, Right = s.Item3 }).Name(nameof(OP));

    public static readonly Deferred<IStatement> Condition = Deferred<IStatement>(nameof(Condition));

    public static readonly Parser<char> ParenOpen = IgnoreSeparator(Char('(')).Name(nameof(ParenOpen));
    public static readonly Parser<char> ParenClose = IgnoreSeparator(Char(')')).Name(nameof(ParenClose));

    public static readonly Parser<IStatement> GroupExpression = Between(ParenOpen, Condition, ParenClose).Name(nameof(GroupExpression));
    public static readonly Parser<IStatement> NotExpression = Between(IgnoreSeparator(Text("!(")), Condition, ParenClose).Then<IStatement>(static s => new UnaryStatement() { Operater = "not", Statement = s }).Name(nameof(NotExpression));

    public static readonly Parser<IStatement> ConditionValue = OP.Or(BoolValue).Name(nameof(ConditionValue));

    public static readonly Parser<IStatement> Primary = NotExpression.Or(GroupExpression).Or(ConditionValue).Name(nameof(Primary));

    public static readonly Parser<IStatement> Conditions = Primary.LeftAssociative(
               (IgnoreSeparator(Text("&&")), static (x, y) => new OperaterStatement() { Left = x, Operater = "and", Right = y }),
               (IgnoreSeparator(Text("and", true)), static (x, y) => new OperaterStatement() { Left = x, Operater = "and", Right = y }),
               (IgnoreSeparator(Text("||")), static (x, y) => new OperaterStatement() { Left = x, Operater = "or", Right = y }),
               (IgnoreSeparator(Text("or", true)), static (x, y) => new OperaterStatement() { Left = x, Operater = "or", Right = y })
           ).Name(nameof(Conditions));

    static TemplateEngineParser()
    {
        ConditionParser = Condition.Parser = Conditions;
        ConditionParser = ConditionParser.Eof();
    }
}