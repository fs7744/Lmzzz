namespace Lmzzz.JsonPath.Statements;

public class RootNode : Node
{
    public override string ToString()
    {
        return Child is not null ? $"$.{Child.ToString()}" : "$";
    }
}