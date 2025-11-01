using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class UnionSelectionStatement : IStatement
{
    public UnionSelectionStatement(List<IStatement> list)
    {
        List = list.ToArray();
    }

    public IStatement[] List { get; }

    public JsonNode? Evaluate(JsonPathContext context)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return $"[{string.Join(",", List.Select(static x => x.ToString()))}]";
    }
}