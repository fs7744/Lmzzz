using Lmzzz.Chars;
using Lmzzz.Chars.Fluent;
using static Lmzzz.Chars.Fluent.Parsers;

namespace Lmzzz.Template.Inner;

public class TemplateEngineParser
{
    public static readonly Parser<IStatement> ConditionParser;
    public static readonly Parser<IValueStatement> NullValue = Text("null").Then<IValueStatement>(static s => NullValueStatement.Value);

    public static readonly Parser<IValueStatement> BoolValue = Text("true", true).Then<IValueStatement>(static s => BoolValueStatement.True)
            .Or(Text("false", true).Then<IValueStatement>(static s => BoolValueStatement.False));

    public static readonly Parser<IValueStatement> NumberValue = Decimal().Then<IValueStatement>(static s => new DecimalValueStatement(s));
    public static readonly Parser<IValueStatement> Field = Separated(Char('.'), Identifier(Character.SVIdentifierStart, Character.SVIdentifierPart)).Then<IValueStatement>(static s => new FieldStatement(s));

    public static readonly Parser<IValueStatement> AnyValue = NullValue.Or(BoolValue).Or(NumberValue).Or(Field);

    public static readonly Parser<IConditionStatement> OP = AnyValue
        .And(Text("==").Or(Text("!=")).Or(Text(">")).Or(Text(">=")).Or(Text("<")).Or(Text("<=")))
        .And(AnyValue)
        .Then<IConditionStatement>(static s => new OperaterStatement() { Left = s.Item1, Operater = s.Item2, Right = s.Item3 });

    public static readonly Deferred<IStatement> Condition = Deferred<IStatement>();

    public static readonly Parser<char> ParenOpen = Char('(');
    public static readonly Parser<char> ParenClose = Char(')');

    public static readonly Parser<IStatement> GroupExpression = Between(ParenOpen, Condition, ParenClose);
    public static readonly Parser<IStatement> NotExpression = Between(Text("!("), Condition, ParenClose).Then<IStatement>(static s => new UnaryStatement() { Operater = "not", Statement = s });

    public static readonly Parser<IStatement> ConditionValue = OP.Then(static s => s as IStatement).Or(BoolValue.Then(static s => s as IStatement));

    public static readonly Parser<IStatement> Primary = NotExpression.Or(GroupExpression).Or(ConditionValue);

    public static readonly Parser<IStatement> Conditions = Primary.LeftAssociative(
               (Text("&&"), static (x, y) => new OperaterStatement() { Left = x, Operater = "and", Right = y }),
               (Text("and", true), static (x, y) => new OperaterStatement() { Left = x, Operater = "and", Right = y }),
               (Text("||"), static (x, y) => new OperaterStatement() { Left = x, Operater = "or", Right = y }),
               (Text("or", true), static (x, y) => new OperaterStatement() { Left = x, Operater = "or", Right = y })
           );

    static TemplateEngineParser()
    {
        ConditionParser = Condition.Parser = Conditions;
        ConditionParser = ConditionParser.Eof().ElseError("error");
    }
}