using Lmzzz.Chars;
using static Lmzzz.Chars.Parsers;

namespace Samples.Json;

public class JsonParser
{
    public static readonly Parser<IJson> Json;

    static JsonParser()
    {
        var lBrace = Char('{');
        var rBrace = Char('}');
        var lBracket = Char('[');
        var rBracket = Char(']');
        var colon = Char(':');
        var comma = Char(',');
        var nullv = Text("null").Then<IJson>(static s => JsonNull.Value);
        var boolv = (Text("true", true).Then<IJson>(static s => JsonBool.True))
                .Or(Text("false", true).Then<IJson>(static s => JsonBool.False));

        var number = Decimal().Then<IJson>(static s => new JsonNumber(s));

        var str = String();

        var jsonString =
            str
                .Then<IJson>(static s => new JsonString(s.ToString()));

        var json = Deferred<IJson>();

        var jsonArray =
            Between(lBracket, Separated(comma, json), rBracket)
                .Then<IJson>(static els => new JsonArray(els));

        var jsonMember =
            str.And(colon).And(json)
                .Then(static member => new KeyValuePair<string, IJson>(member.Item1.ToString(), member.Item3));

        var jsonObject =
            Between(lBrace, Separated(comma, jsonMember), rBrace)
                .Then<IJson>(static kvps => new JsonObject(new Dictionary<string, IJson>(kvps)));

        Json = json.Parser = jsonString.Or(jsonArray).Or(jsonObject).Or(nullv).Or(boolv).Or(number);
        Json = Json.Eof();
    }

    public static IJson Parse(string input)
    {
        if (Json.TryParse(input, out var result, out _))
        {
            return result;
        }
        else
        {
            return null;
        }
    }
}