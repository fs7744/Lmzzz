namespace Lmzzz.Template.Inner;

public class IfConditionStatement : IStatement
{
    private IStatement condition;
    private IStatement text;

    public IfConditionStatement(IStatement condition, IStatement text)
    {
        this.condition = condition;
        this.text = text;
    }

    public object? Evaluate(TemplateContext context)
    {
        if (true.Equals(condition.Evaluate(context)))
        {
            text.Evaluate(context);
            return true;
        }
        return false;
    }
}