namespace Lmzzz.JsonPath.Statements;

public class Node : IStatement
{
    public IStatement? Child { get; set; }
}

public class RootNode : Node
{
}

public class CurrentNode : Node
{
}

public class Member : IStatement
{
    public string Name { get; set; }
}

public class NumberValue : IStatement
{
    public NumberValue(decimal x)
    {
        this.Value = x;
    }

    public decimal Value { get; }
}

public class BoolValue : IStatement
{
    public static readonly BoolValue True = new(true);
    public static readonly BoolValue False = new(false);

    public BoolValue(bool x)
    {
        this.Value = x;
    }

    public bool Value { get; }

    public static BoolValue From(bool x) => x ? True : False;
}

public class NullValue : IStatement
{
    public static readonly NullValue Value = new();
}

public class StringValue : IStatement
{
    public StringValue(string value)
    {
        Value = value;
    }

    public string Value { get; }
}

public class OperatorStatement : IStatement
{
    public IStatement Left { get; set; }
    public string Operator { get; set; }
    public IStatement Right { get; set; }
}

public class UnaryOperaterStatement : IStatement
{
    public string Operator { get; set; }
    public IStatement Statement { get; set; }
}