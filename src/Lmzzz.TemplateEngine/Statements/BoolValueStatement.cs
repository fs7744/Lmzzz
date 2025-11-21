namespace Lmzzz.Template.Inner;

public class BoolValueStatement : IValueStatement, IConditionStatement
{
    public static readonly BoolValueStatement True = new BoolValueStatement(true);
    public static readonly BoolValueStatement False = new BoolValueStatement(false);

    public BoolValueStatement(bool value)
    {
        this.Value = value;
    }

    public bool Value { get; }

    public object? Evaluate(TemplateContext context)
    {
        return Value;
    }

    public bool EvaluateCondition(TemplateContext context)
    {
        return Value;
    }

    public void Visit(Action<IStatement> visitor)
    {
    }
}