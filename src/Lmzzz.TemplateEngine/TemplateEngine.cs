using Lmzzz.Chars.Fluent;
using static Lmzzz.Chars.Fluent.Parsers;

namespace Lmzzz;

public class TemplateEngine
{
    private static readonly Parser<IConditionStatement> ConditionParser;

    static TemplateEngine()
    {
        var nullValue = Text("null").Then<IValueStatement>(static s => NullValueStatement.Value);
        var boolValue = Text("true", true).Then<IValueStatement>(static s => BoolValueStatement.True)
            .Or(Text("false", true).Then<IValueStatement>(static s => BoolValueStatement.False));
        var numberValue = Decimal().Then<IValueStatement>(static s => new DecimalValueStatement(s));

        var anyValue = nullValue.Or(boolValue).Or(numberValue);

        var field = Identifier(Character.SVIdentifierStart, Character.SVIdentifierPart).Then(static s => new FieldStatement(s.ToString()));
        var dot = Char('.');
        var condition = Deferred<IConditionStatement>();
        var parenOpen = Char('(');
        var parenClose = Char(')');

        var and = condition.And(Text("&&").Or(Text("and", true))).And(condition).Then(static s => new OperaterStatement() { Left = s.Item1, Operater = s.Item2, Right = s.Item3 });
        var or = condition.And(Text("||").Or(Text("or", true))).And(condition).Then(static s => new OperaterStatement() { Left = s.Item1, Operater = s.Item2, Right = s.Item3 });

        //ConditionParser = condition.Parser = boolValue.Or()
    }
}