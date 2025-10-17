using Lmzzz.Chars;
using Lmzzz.Chars.Fluent;
using static Lmzzz.Chars.Fluent.Parsers;

namespace Lmzzz.Template.Inner;

public class TemplateEngineParser
{
    public static readonly Parser<IStatement> ConditionParser;
    public static readonly Parser<IStatement> NullValue = Text("null").Then<IStatement>(static s => NullValueStatement.Value);

    public static readonly Parser<IStatement> BoolValue = Text("true", true).Then<IStatement>(static s => BoolValueStatement.True)
            .Or(Text("false", true).Then<IStatement>(static s => BoolValueStatement.False));

    public static readonly Parser<IStatement> NumberValue = Decimal().Then<IStatement>(static s => new DecimalValueStatement(s));
    public static readonly Parser<IStatement> Field = Separated(Char('.'), Identifier(Character.SVIdentifierStart, Character.SVIdentifierPart)).Then<IStatement>(static s => new FieldStatement(s));

    public static readonly Parser<IStatement> AnyValue = NullValue.Or(BoolValue).Or(NumberValue).Or(Field);

    public static readonly Parser<IStatement> OP = AnyValue
        .And(Text("==").Or(Text("!=")).Or(Text(">")).Or(Text(">=")).Or(Text("<")).Or(Text("<=")))
        .And(AnyValue)
        .Then<IStatement>(static s => new OperaterStatement() { Left = s.Item1, Operater = s.Item2, Right = s.Item3 });

    public static readonly Deferred<IStatement> Condition = Deferred<IStatement>();

    public static readonly Parser<char> ParenOpen = Char('(');
    public static readonly Parser<char> ParenClose = Char(')');

    public static readonly Parser<IStatement> GroupExpression = Between(ParenOpen, Condition, ParenClose);
    public static readonly Parser<IStatement> NotExpression = Between(Text("!("), Condition, ParenClose).Then<IStatement>(static s => new UnaryStatement() { Operater = "not", Statement = s });

    public static readonly Parser<IStatement> ConditionValue = OP.Or(BoolValue);

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
        ConditionParser = ConditionParser.Eof().ElseError("Wrong syntax");
    }
}