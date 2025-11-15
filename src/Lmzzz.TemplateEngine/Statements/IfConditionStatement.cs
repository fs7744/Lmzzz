namespace Lmzzz.Template.Inner;

public class IfConditionStatement : IConditionStatement
{
    private IConditionStatement condition;
    private IStatement text;

    public IfConditionStatement(IConditionStatement condition, IStatement text)
    {
        this.condition = condition;
        this.text = text;
    }

    public object? Evaluate(TemplateContext context)
    {
        return EvaluateCondition(context);
    }

    public bool EvaluateCondition(TemplateContext context)
    {
        if (condition.EvaluateCondition(context))
        {
            text.Evaluate(context);
            return true;
        }
        return false;
    }
}