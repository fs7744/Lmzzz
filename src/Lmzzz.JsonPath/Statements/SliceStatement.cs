namespace Lmzzz.JsonPath.Statements;

public class SliceStatement : IStatement
{
    public int? Start { get; set; }
    public int? End { get; set; }
    public int? Step { get; set; }
}