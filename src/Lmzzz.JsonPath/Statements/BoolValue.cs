namespace Lmzzz.JsonPath.Statements;

public class BoolValue : IStatementValue
{
    public static readonly BoolValue True = new(true);
    public static readonly BoolValue False = new(false);

    public BoolValue(bool x)
    {
        this.Value = x;
    }

    public bool Value { get; }

    object IStatementValue.Value => Value;

    public static BoolValue From(bool x) => x ? True : False;

    public override string ToString()
    {
        return Value.ToString();
    }
}