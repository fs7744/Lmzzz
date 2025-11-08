namespace Lmzzz.Template.Inner;

public class DecimalValueStatement : IValueStatement
{
    public DecimalValueStatement(decimal value)
    {
        this.Value = value;
    }

    public decimal Value { get; }

    public object? Evaluate(TemplateContext context)
    {
        return Value;
    }
}