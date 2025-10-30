namespace Lmzzz.JsonPath.Statements;

public class StringValue : IStatementValue
{
    public StringValue(string value)
    {
        Value = value;
    }

    public string Value { get; }

    object IStatementValue.Value => Value;

    public override string ToString()
    {
        return Value;
    }
}