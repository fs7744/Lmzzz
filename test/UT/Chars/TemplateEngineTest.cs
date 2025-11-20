using Lmzzz.Chars.Fluent;
using Lmzzz.Template;
using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace UT.Chars;

public class TemplateEngineTest
{
    private A data = new A
    {
        Int = 4,
        D = 5.5,
        IntD = new Dictionary<string, int>() { { "a99", 44 }, { "99", 144 } },
        Array = [2, 34, 55],
        List = [2, 34, 55],
        LinkedList = new LinkedList<int>([2, 34, 55]) { },
        Str = "9sd",
        ArrayNull = [null, new { a = new { a = 333 } }]
    };

    public TemplateEngineTest()
    {
        data.HttpContext = new DefaultHttpContext();
        var req = data.HttpContext.Request;
        req.Path = "/testp/dsd/fsdfx/fadasd3/中";
        req.Method = "GET";
        req.Host = new HostString("x.com");
        req.Scheme = "https";
        req.Protocol = "HTTP/1.1";
        req.ContentType = "json";
        req.QueryString = new QueryString("?s=123&d=456&f=789");
        req.IsHttps = true;
        for (int i = 0; i < 10; i++)
        {
            req.Headers.Add($"x-{i}", new string[] { $"v-{i}", $"x-{i}", $"s-{i}" });
        }
        EqualStatement.EqualityComparers[typeof(PathString)] = (l, r) =>
        {
            if (l is PathString pl)
            {
                if (pl.HasValue)
                {
                    if (r is PathString pr)
                    {
                        return pl == pr;
                    }
                    else if (r is string rs)
                    {
                        return pl.Value == rs;
                    }
                }
                else
                    return false;
            }
            return false;
        };
    }

    [Theory]
    [InlineData("true == 1")]
    [InlineData("true")]
    [InlineData("1 != false")]
    [InlineData("false")]
    [InlineData("(true == 1)")]
    [InlineData("(true)")]
    [InlineData("(1 != false)")]
    [InlineData("(false)")]
    [InlineData("(false) and true")]
    [InlineData("1 != 2.3")]
    [InlineData("2 > s")]
    [InlineData("(s > f(2))")]
    [InlineData("1 != 2.3 and 2 > s.d.d")]
    [InlineData("1 != 2.3 and (2 > s.d.d and false)")]
    [InlineData("1 != 2.3 or 2 > s.d.d")]
    [InlineData("(1 != 2.3 or (2 > s.d.d and 6 != 7)) and 3 !=4")]
    [InlineData("(1 != f(2.3) or (2 > s.d.d and 6 != 7)) and 3 !=4")]
    [InlineData("(1 != f(2.3) && z() || true || f(s.d))")]
    [InlineData("! (1 != f(2.3) && z() || true || f(s.d))")]
    public void ConditionParserTest(string text)
    {
        var r = TemplateEngineParser.ConditionParser.TryParse(text, out var v, out var err);
        Assert.True(r);
        Assert.NotNull(v);
    }

    [Theory]
    [InlineData("f(2)")]
    [InlineData("f()")]
    [InlineData("f")]
    public void AnyValueTest(string text)
    {
        var r = TemplateEngineParser.AnyValue.Eof().TryParse(text, out var v, out var err);
        Assert.True(r);
        Assert.NotNull(v);
    }

    [Theory]
    [InlineData("s > f(2)")]
    public void OPTest(string text)
    {
        var r = TemplateEngineParser.OP.Eof().TryParse(text, out var v, out var err);
        Assert.True(r);
        Assert.NotNull(v);
    }

    [Theory]
    [InlineData("f(2)")]
    [InlineData("f(s.d)")]
    [InlineData("z()")]
    [InlineData("Regex(Str,'^9.*')")]
    public void FunctionExprTest(string text)
    {
        var r = TemplateEngineParser.FunctionExpr.Eof().TryParse(text, out var v, out var err);
        Assert.True(r);
        Assert.NotNull(v);
    }

    [Theory]
    [InlineData("f(2)")]
    [InlineData("f(s.d)")]
    [InlineData("z()")]
    [InlineData("Regex(Str,'^9.*')")]
    public void ConditionValueTest(string text)
    {
        var r = TemplateEngineParser.ConditionValue.Eof().TryParse(text, out var v, out var err);
        Assert.True(r);
        Assert.NotNull(v);
    }

    [Theory]
    [InlineData("true == 1 and [] !=s")]
    public void ConditionParserError(string text)
    {
        var r = TemplateEngineParser.ConditionParser.TryParse(text, out var v, out var err);
        Assert.False(r);
        Assert.NotNull(err);
        Assert.NotNull(err.Position.ToString());
    }

    [Theory]
    [InlineData("Int", 4)]
    [InlineData("D", 5.5)]
    [InlineData("IntDNull.a99", null)]
    [InlineData("IntD.a99", 44)]
    [InlineData("IntD.['a99']", 44)]
    [InlineData("['IntD'].['a99']", 44)]
    [InlineData("IntD.99", 144)]
    [InlineData("Array.0", 2)]
    [InlineData("ArrayNull.0", null)]
    [InlineData("ArrayNull.0.a", null)]
    [InlineData("ArrayNull.1.a.a", 333)]
    [InlineData("Array.1", 34)]
    [InlineData("Array.2", 55)]
    [InlineData("Array.3", null)]
    [InlineData("Array.90", null)]
    [InlineData("List.0", 2)]
    [InlineData("List.1", 34)]
    [InlineData("List.2", 55)]
    [InlineData("List.3", null)]
    [InlineData("List.90", null)]
    [InlineData("LinkedList.0", null)]
    [InlineData("LinkedList.1", null)]
    [InlineData("LinkedList.2", null)]
    [InlineData("LinkedList.3", null)]
    [InlineData("LinkedList.90", null)]
    [InlineData("Array.Length", 3)]
    public void FieldRuntimeModeTest(string text, object d)
    {
        var r = TemplateEngineParser.Field.Eof().TryParse(text, out var v, out var err);
        Assert.True(r);
        var dd = new TemplateContext(data);
        var f = v.Evaluate(dd);
        Assert.Equal(d, f);
    }

    [Theory]
    [InlineData("Int", 4)]
    [InlineData("D", 5.5)]
    [InlineData("IntDNull.a99", null)]
    [InlineData("IntD.a99", 44)]
    [InlineData("IntD.99", 144)]
    [InlineData("Array.0", 2)]
    [InlineData("ArrayNull.0", null)]
    [InlineData("ArrayNull.0.a", null)]
    [InlineData("ArrayNull.1.a.a", null)]
    [InlineData("Array.1", 34)]
    [InlineData("Array.2", 55)]
    [InlineData("Array.3", null)]
    [InlineData("Array.90", null)]
    [InlineData("List.0", 2)]
    [InlineData("List.1", 34)]
    [InlineData("List.2", 55)]
    [InlineData("List.3", null)]
    [InlineData("List.90", null)]
    [InlineData("LinkedList.0", null)]
    [InlineData("LinkedList.1", null)]
    [InlineData("LinkedList.2", null)]
    [InlineData("LinkedList.3", null)]
    [InlineData("LinkedList.90", null)]
    [InlineData("Array.Length", 3)]
    public void FieldDefinedModeTest(string text, object d)
    {
        var r = TemplateEngineParser.Field.Eof().TryParse(text, out var v, out var err);
        Assert.True(r);
        var dd = new TemplateContext(data) { FieldMode = FieldStatementMode.Defined };
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
    [InlineData("HttpContext.Request.Path == '/testp/dsd/fsdfx/fadasd3/中'", true)]
    [InlineData("'/testp/dsd/fsdfx/fadasd3/中' == HttpContext.Request.Path", true)]
    [InlineData("'/testp/dsd/fsdfx/fadasd3/中' == ['HttpContext'].['Request'].['Path']", true)]
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
    [InlineData("{{ xx }}", "")]
    public void ReplaceStrTest(string text, string d)
    {
        var r = TemplateEngineParser.ReplaceStr.Eof().TryParse(text, out var v, out var err);
        Assert.True(r);
        Assert.NotNull(v);
        var dd = new TemplateContext(data) { StringBuilder = new System.Text.StringBuilder() };
        v.Evaluate(dd);
        Assert.Equal(d, dd.StringBuilder.ToString());
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
        //var r = TemplateEngineParser.TemplateValue.Eof().TryParse(text, out var v, out var err);
        //Assert.True(r);
        //Assert.NotNull(v);
        //var dd = new TemplateContext(data) { StringBuilder = new System.Text.StringBuilder() };
        //v.Evaluate(dd);
        //Assert.Equal(d, dd.StringBuilder.ToString());
    }

    [Theory]
    [InlineData("{{ if(4 == Int)}} xx {{endif}}", " xx ")]
    [InlineData("{{ if(4 == Int)}}{{ Int }} xx {{endif}}", "4 xx ")]
    [InlineData("{{ if(4 == Int)}}{{ if(4 == Int)}}{{ Int }}dd{{endif}} xx {{ if(5 == Int)}}{{ Int }}yy{{endif}}{{endif}}", "4dd xx ")]
    [InlineData("{{ if(4 == Int)}}{{ if(5 == Int)}}{{ Int }}dd{{endif}} xx {{ if(4 == Int)}}{{ Int }}yy{{endif}}{{endif}}", " xx 4yy")]
    public void IfEvaluateTest(string text, string d)
    {
        var r = TemplateEngineParser.If.Eof().TryParse(text, out var v, out var err);
        Assert.True(r);
        Assert.NotNull(v);
        var dd = new TemplateContext(data) { StringBuilder = new System.Text.StringBuilder() };
        v.Evaluate(dd);
        Assert.Equal(d, dd.StringBuilder.ToString());
    }

    [Theory]
    [InlineData("{{ for(_v,_i in Array)}} {{_i}}:{{_v}},{{endfor}}", " 0:2, 1:34, 2:55,")]
    public void ForEvaluateTest(string text, string d)
    {
        var r = TemplateEngineParser.For.Eof().TryParse(text, out var v, out var err);
        Assert.True(r);
        Assert.NotNull(v);
        var dd = new TemplateContext(data) { StringBuilder = new System.Text.StringBuilder() };
        v.Evaluate(dd);
        Assert.Equal(d, dd.StringBuilder.ToString());
    }
}

public class A
{
    public double? Dd;
    public double? D;
    public int Int { get; set; }
    public string Str { get; set; }
    public Dictionary<string, int> IntD { get; set; }

    public int[] Array { get; set; }

    public List<int> List { get; set; }

    public LinkedList<int> LinkedList { get; set; }

    public HttpContext HttpContext { get; set; }

    public Dictionary<string, int> IntDNull { get; set; }
    public object[] ArrayNull { get; set; }
}