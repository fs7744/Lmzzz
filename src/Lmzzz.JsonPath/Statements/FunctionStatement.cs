namespace Lmzzz.JsonPath.Statements;

internal class FunctionStatement : IStatement
{
    public string Name { get; set; }
    public IStatement[] Arguments { get; set; }
}