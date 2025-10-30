namespace Lmzzz.JsonPath.Statements;

public class IndexSelectorStatment : IStatement
{
    public int Index { get; set; }

    public override string ToString()
    {
        return $"[{Index}]";
    }
}