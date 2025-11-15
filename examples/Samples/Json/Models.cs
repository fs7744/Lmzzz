namespace Samples.Json;

public interface IJson
{
}

public class JsonArray : IJson
{
    public IReadOnlyList<IJson> Elements { get; }

    public JsonArray(IReadOnlyList<IJson> elements)
    {
        Elements = elements;
    }

    public override string ToString()
        => $"[{string.Join(",", Elements.Select(e => e.ToString()))}]";
}

public class JsonObject : IJson
{
    public IDictionary<string, IJson> Members { get; }

    public JsonObject(IDictionary<string, IJson> members)
    {
        Members = members;
    }

    public override string ToString()
        => $"{{{string.Join(",", Members.Select(kvp => $"\"{kvp.Key}\":{kvp.Value}"))}}}";
}

public class JsonString : IJson
{
    public string Value { get; }

    public JsonString(string value)
    {
        Value = value;
    }

    public override string ToString()
        => $"\"{Value}\"";
}

public class JsonNumber : IJson
{
    public decimal Value { get; }

    public JsonNumber(decimal value)
    {
        Value = value;
    }

    public override string ToString()
        => $"{Value}";
}

public class JsonBool : IJson
{
    public static readonly JsonBool True = new JsonBool(true);
    public static readonly JsonBool False = new JsonBool(false);
    public bool Value { get; }

    public JsonBool(bool value)
    {
        Value = value;
    }

    public override string ToString()
        => $"{Value}";
}

public class JsonNull : IJson
{
    public static readonly JsonNull Value = new JsonNull();

    public override string ToString()
        => "null";
}