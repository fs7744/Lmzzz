namespace Lmzzz.Template.Inner;

public class IfStatement : IStatement
{
    public IfConditionStatement If { get; set; }
    public IEnumerable<IfConditionStatement> ElseIfs { get; set; }
    public IStatement? Else { get; set; }

    public object? Evaluate(TemplateContext context)
    {
        if (If.EvaluateCondition(context))
        {
        }
        else if (ElseIfs != null && ElseIfs.Any(x => x.EvaluateCondition(context)))
        { }
        else if (Else != null)
            Else.Evaluate(context);

        return null;
    }

    public void Visit(Action<IStatement> visitor)
    {
        if (If is not null)
            visitor(If);
        if (ElseIfs is not null)
            foreach (var item in ElseIfs)
            {
                if (item is not null)
                    visitor(item);
            }
        if (Else is not null)
            visitor(Else);
    }
}