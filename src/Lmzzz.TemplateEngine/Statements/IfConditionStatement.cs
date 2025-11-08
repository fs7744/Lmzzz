namespace Lmzzz.Template.Inner;

public class IfConditionStatement : IStatement
{
    private IStatement condition;
    private string text;

    public IfConditionStatement(IStatement condition, string text)
    {
        this.condition = condition;
        this.text = text;
    }

    public object? Evaluate(TemplateContext context)
    {
        if (true.Equals(condition.Evaluate(context)))
        {
            context.StringBuilder.Append(text);
            return true;
        }
        return false;
    }
}