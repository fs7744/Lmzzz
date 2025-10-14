namespace Lmzzz;

public class BoolValueStatement : IValueStatement, IConditionStatement
{
    public static readonly BoolValueStatement True = new BoolValueStatement(true);
    public static readonly BoolValueStatement False = new BoolValueStatement(false);

    public BoolValueStatement(bool value)
    {
        this.Value = value;
    }

    public bool Value { get; }
}