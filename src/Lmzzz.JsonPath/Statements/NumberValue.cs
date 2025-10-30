namespace Lmzzz.JsonPath.Statements;

public class NumberValue : IStatementValue
{
    public NumberValue(decimal x)
    {
        this.Value = x;
    }

    public decimal Value { get; }

    object IStatementValue.Value => Value;

    public override string ToString()
    {
        return Value.ToString();
    }
}