namespace Lmzzz.JsonPath.Statements;

public class NullValue : IStatementValue
{
    public static readonly NullValue Value = new();

    object IStatementValue.Value => null;
}
