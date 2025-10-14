namespace Lmzzz;

public class DecimalValueStatement : IValueStatement
{
    public DecimalValueStatement(decimal value)
    {
        this.Value = value;
    }

    public decimal Value { get; }
}