namespace Lmzzz.JsonPath.Statements;

public class FunctionStatement : IStatement
{
    public string Name { get; set; }
    public IStatement[] Arguments { get; set; }

    public override string ToString()
    {
        return $"{Name}({string.Join(",", Arguments.Select(static x => x.ToString()))})";
    }
}