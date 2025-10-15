namespace Lmzzz;

public interface IConditionStatement : IStatement
{
}

public class OperaterStatement : IConditionStatement
{
    public IStatement Left { get; set; }
    public string Operater { get; set; }
    public IStatement Right { get; set; }
}

public class UnaryStatement : IConditionStatement
{
    public IStatement Statement { get; set; }
    public string Operater { get; set; }
}