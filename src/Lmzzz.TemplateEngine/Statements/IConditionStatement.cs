namespace Lmzzz.Template.Inner;

public interface IConditionStatement : IStatement
{
    public bool EvaluateCondition(TemplateContext context);
}

public interface IOperaterStatement : IConditionStatement
{
    public IStatement Left { get; }
    public string Operater { get; }
    public IStatement Right { get; }
}

public interface IUnaryStatement : IConditionStatement
{
    public IStatement Statement { get; }
    public string Operater { get; }
}