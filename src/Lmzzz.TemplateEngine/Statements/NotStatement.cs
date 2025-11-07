namespace Lmzzz.Template.Inner;

public class NotStatement : IUnaryStatement
{
    public NotStatement(IStatement statement)
    {
        this.Statement = statement;
    }

    public IStatement Statement { get; }

    public string Operater => "!";
}