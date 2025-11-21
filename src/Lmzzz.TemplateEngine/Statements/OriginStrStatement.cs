namespace Lmzzz.Template.Inner;

public class OriginStrStatement : IStatement
{
    public string Text { get; set; }

    public OriginStrStatement(string text)
    {
        this.Text = text;
    }

    public object? Evaluate(TemplateContext context)
    {
        context.StringBuilder.Append(Text);
        return null;
    }

    public void Visit(Action<IStatement> visitor)
    {
    }
}