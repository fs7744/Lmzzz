namespace Lmzzz.JsonPath.Statements;

internal class AndStatement : IStatement
{
    public IStatement Left { get; set; }
    public IStatement Right { get; set; }
}