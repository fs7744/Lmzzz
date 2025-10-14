namespace Lmzzz;

public interface IConditionStatement
{
}

public class OperaterStatement : IConditionStatement
{
    public IConditionStatement Left { get; set; }
    public string Operater { get; set; }
    public IConditionStatement Right { get; set; }
}