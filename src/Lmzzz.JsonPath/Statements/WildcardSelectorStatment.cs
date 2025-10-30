namespace Lmzzz.JsonPath.Statements;

public class WildcardSelectorStatment : IStatement
{
    public static readonly WildcardSelectorStatment Value = new();

    public override string ToString()
    {
        return "*";
    }
}