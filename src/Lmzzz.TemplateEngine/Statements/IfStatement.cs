namespace Lmzzz.Template.Inner;

public class IfStatement : IStatement
{
    public IfConditionStatement If { get; set; }
    public IEnumerable<IfConditionStatement> ElseIfs { get; set; }
    public string? Else { get; set; }

    public object? Evaluate(TemplateContext context)
    {
        if (true.Equals(If.Evaluate(context)))
        {
        }
        else if (ElseIfs != null && ElseIfs.Any(x => true.Equals(x.Evaluate(context))))
        { }
        else if (Else != null)
            context.StringBuilder.Append(Else);

        return null;
    }
}