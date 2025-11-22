using Lmzzz.Chars.Fluent;
using Lmzzz.Template;
using Lmzzz.Template.Inner;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace UT.Chars;

public class JsonDataTemplateEngineTest
{
    private object data = JsonNode.Parse(JsonSerializer.Serialize(new
    {
        Int = 4,
        D = 5.5,
        IntD = new Dictionary<string, int>() { { "a99", 44 }, { "99", 144 } },
        Array = new int[] { 2, 34, 55 },
        List = new int[] { 2, 34, 55 },
        LinkedList = new LinkedList<int>([2, 34, 55]) { },
        Str = "9sd",
        ArrayNull = new object[] { null, new { a = new { a = 333 } } }
    }));

    [Theory]
    [InlineData("Int", 4d)]
    [InlineData("D", 5.5)]
    [InlineData("IntDNull.a99", null)]
    [InlineData("IntD.a99", 44d)]
    [InlineData("IntD.['a99']", 44d)]
    [InlineData("['IntD'].['a99']", 44d)]
    [InlineData("IntD.99", 144d)]
    [InlineData("Array.0", 2d)]
    [InlineData("ArrayNull.0", null)]
    [InlineData("ArrayNull.0.a", null)]
    [InlineData("ArrayNull.1.a.a", 333d)]
    [InlineData("Array.1", 34d)]
    [InlineData("Array.2", 55d)]
    [InlineData("Array.3", null)]
    [InlineData("Array.90", null)]
    [InlineData("List.0", 2d)]
    [InlineData("List.1", 34d)]
    [InlineData("List.2", 55d)]
    [InlineData("List.3", null)]
    [InlineData("List.90", null)]
    [InlineData("LinkedList.0", 2d)]
    [InlineData("LinkedList.1", 34d)]
    [InlineData("LinkedList.2", 55d)]
    [InlineData("LinkedList.3", null)]
    [InlineData("LinkedList.90", null)]
    [InlineData("Array.Count", null)]
    public void FieldRuntimeModeTest(string text, double? di)
    {
        decimal? d = di.HasValue ? new decimal(di.Value) : null;
        var r = TemplateEngineParser.Field.Eof().TryParse(text, out var v, out var err);
        Assert.True(r);
        var dd = new TemplateContext(data);
        var f = v.Evaluate(dd);
        Assert.Equal(d, f);
    }

    [Theory]
    [InlineData("4 == Int", true)]
    [InlineData("Int == 4", true)]
    [InlineData("D == null", false)]
    [InlineData("D == 5.5", true)]
    [InlineData("Dd == 5.5", false)]
    [InlineData("Dd == Dd", true)]
    [InlineData("4 != Int", false)]
    [InlineData("Int != 4", false)]
    [InlineData("D != null", true)]
    [InlineData("D != 5.5", false)]
    [InlineData("Dd != 5.5", true)]
    [InlineData("Dd != Dd", false)]
    [InlineData("D >= 5.5", true)]
    [InlineData("D > 5.5", false)]
    [InlineData("D <= 5.5", true)]
    [InlineData("D < 5.5", false)]
    [InlineData("!(D < 5.5)", true)]
    [InlineData("!(null == null)", false)]
    [InlineData("!(null != null)", true)]
    [InlineData("!(null != null) and 1 == 3", false)]
    [InlineData("!(null != null) && 1 == 3", false)]
    [InlineData("!(null != null) || 1 == 3", true)]
    [InlineData("!(null != null) or 1 == 3", true)]
    [InlineData("Regex(Str,'^9.*')", true)]
    [InlineData("Regex ( Str, '^7.*', 'ECMAScript')", false)]
    public void ConditionEvaluateTest(string text, object d)
    {
        var r = TemplateEngineParser.ConditionParser.TryParse(text, out var v, out var err);
        Assert.True(r);
        Assert.NotNull(v);
        var dd = new TemplateContext(data);
        var f = v.Evaluate(dd);
        Assert.Equal(d, f);
    }

    [Theory]
    [InlineData(" xx ", " xx ")]
    [InlineData("{{ xx }}", "")]
    [InlineData("{{ if(4 == Int)}} xx {{endif}}", " xx ")]
    [InlineData("{{ if(4 == Int)}}{{ Int }} xx {{endif}}", "4 xx ")]
    [InlineData("{{ if(4 == Int)}}{{ if(4 == Int)}}{{ Int }}dd{{endif}} xx {{ if(5 == Int)}}{{ Int }}yy{{endif}}{{endif}}", "4dd xx ")]
    [InlineData("{{ if(4 == Int)}}{{ for(_v,_i in Array)}} {{_i}}:{{_v}},{{endfor}}{{endif}}", " 0:2, 1:34, 2:55,")]
    public void TemplateValueTest(string text, string d)
    {
        Assert.Equal(d, text.EvaluateTemplate(data));
    }
}
