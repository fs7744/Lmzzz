namespace Lmzzz.Template.Inner;

public class FunctionStatement : IStatement
{
    public string Name { get; set; }
    public IStatement[] Arguments { get; set; }
}