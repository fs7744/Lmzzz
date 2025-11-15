using Lmzzz.Template.Inner;

namespace Lmzzz;

public interface IStatement
{
    public object? Evaluate(TemplateContext context);
}