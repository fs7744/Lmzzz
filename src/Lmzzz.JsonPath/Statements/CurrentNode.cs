namespace Lmzzz.JsonPath.Statements;

public class CurrentNode : Node
{
    public override string ToString()
    {
        return Child is not null ? $"@.{Child.ToString()}" : "@";
    }
}