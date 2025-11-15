namespace Lmzzz.Template.Inner;

public class StatementUtils
{
    public static IConditionStatement Create(string operater, IStatement left, IStatement right)
    {
        switch (operater)
        {
            case "==":
                return new EqualStatement(left, right);

            case "!=":
                return new NotEqualStatement(left, right);

            case ">":
                return new GreaterThenStatement(left, right);

            case ">=":
                return new GreaterThenAndEqualStatement(left, right);

            case "<":
                return new LessThenStatement(left, right);

            case "<=":
                return new LessThenAndEqualStatement(left, right);

            case "&&":
                return new AndStatement(left, right);

            case "||":
                return new OrStatement(left, right);

            default:
                throw new NotSupportedException(operater);
        }
    }

    public static IConditionStatement Create(string operater, IStatement statement)
    {
        switch (operater)
        {
            case "!":
                return new NotStatement(statement);

            default:
                throw new NotSupportedException(operater);
        }
    }
}