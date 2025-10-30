namespace Lmzzz.JsonPath.Statements;

public class UnionSelectionStatement : IStatement
{
    public UnionSelectionStatement(List<IStatement> list)
    {
        List = list.ToArray();
    }

    public IStatement[] List { get; }
}