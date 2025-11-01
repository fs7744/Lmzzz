using System.Text.Json.Nodes;

namespace Lmzzz.JsonPath;

public class JsonPathContext
{
    public JsonNode Root { get; set; }
    public JsonNode? Current { get; set; }
}