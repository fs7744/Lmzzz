namespace Lmzzz;

public interface IValueStatement : IStatement
{
}

public interface IFieldStatement : IValueStatement
{
}

public class FieldStatement : IFieldStatement
{
    public string Name { get; }

    public FieldStatement(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}