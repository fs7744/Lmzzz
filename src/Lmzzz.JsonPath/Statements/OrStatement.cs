namespace Lmzzz.JsonPath.Statements;

internal class OrStatement : IStatement
{
    public IStatement Left { get; set; }
    public IStatement Right { get; set; }
}