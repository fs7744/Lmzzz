namespace Lmzzz.JsonPath.Statements;

internal class UnionSelectionStatement : IStatement
{
    public UnionSelectionStatement(List<IStatement> list)
    {
        List = list.ToArray();
    }

    public IStatement[] List { get; }
}