using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath.Statements;

public class UnionSelectionStatement : IParentStatement
{
    public IStatement? Child { get; set; }

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
        return Child.ToChildString($"[{string.Join(",", List.Select(static x => x.ToString()))}]");
    }
}