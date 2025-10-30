using Lmzzz.JsonPath;
using Lmzzz.Chars.Fluent;
using Lmzzz.JsonPath.Statements;

namespace UT.JsonPath;

public class JsonPathParserTest
{
    [Theory]
    [InlineData("sss", true, "sss")]
    [InlineData("sss.s", true, "sss")]
    [InlineData("dsd[sd]csd", true, "dsd")]
    [InlineData("[sd]csd", false, "")]
    [InlineData(",sd]csd", false, "")]
    [InlineData("]sd]csd", false, "")]
    [InlineData("(sd]csd", false, "")]
    [InlineData(")sd]csd", false, "")]
    [InlineData(".sd]csd", false, "")]
    [InlineData("\"sd]csd", false, "")]
    [InlineData("'sd]csd", false, "")]
    public void MemberNameShorthandTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.MemberNameShorthand;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, (v as Member).Name);
    }

    [Theory]
    [InlineData("1.23", true, 1.23)]
    [InlineData("-0.55", true, -0.55)]
    [InlineData("9e-4", true, 0.0009)]
    [InlineData("[sd]csd", false, 0)]
    [InlineData("--3", false, 0)]
    [InlineData("+3", false, 0)]
    public void NumTest(string test, bool r, decimal rr)
    {
        var p = JsonPathParser.Num;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, (v as NumberValue).Value);
    }

    [Theory]
    [InlineData("123", true, 123)]
    [InlineData("1.23", true, 1)]
    [InlineData("-0.55", true, -0)]
    [InlineData("9e-4", true, 9)]
    [InlineData("[sd]csd", false, 0)]
    [InlineData("--3", false, 0)]
    [InlineData("+3", false, 0)]
    public void IntTest(string test, bool r, decimal rr)
    {
        var p = JsonPathParser.Int;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, v);
    }

    [Theory]
    [InlineData("\"ss\\\"s\"", true, "ss\\\"s")]
    [InlineData("\"ss\r\ns\"", true, "ss\r\ns")]
    [InlineData("\"sss\"", true, "sss")]
    [InlineData("'sss'", true, "sss")]
    [InlineData("sss", false, "")]
    public void StringLiteralTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.StringLiteral;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, v.Span.ToString());
    }

    [Theory]
    [InlineData("\"ss\r\ns\"", true, "ss\r\ns")]
    [InlineData("\"sss\"", true, "sss")]
    [InlineData("'sss'", true, "sss")]
    [InlineData("true", true, true)]
    [InlineData("false", true, false)]
    [InlineData("null", true, null)]
    [InlineData("sss", false, "")]
    public void LiteralTest(string test, bool r, object rr)
    {
        var p = JsonPathParser.Literal;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, (v as IStatementValue).Value);
    }

    [Theory]
    [InlineData("$", true, "$")]
    [InlineData("sss", false, "")]
    public void RootIdentifierTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.RootIdentifier;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, v.ToString());
    }

    [Theory]
    [InlineData("@", true, "@")]
    [InlineData("sss", false, "")]
    public void CurrentNodeIdentifierTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.CurrentNodeIdentifier;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, v.ToString());
    }

    [Theory]
    [InlineData("==", true, "==")]
    [InlineData("!=", true, "!=")]
    [InlineData("<=", true, "<=")]
    [InlineData("<", true, "<")]
    [InlineData(">=", true, ">=")]
    [InlineData(">", true, ">")]
    [InlineData("sss", false, "")]
    public void ComparisonOpTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.ComparisonOp;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, v);
    }

    [Theory]
    [InlineData(": ", true, null, null, null)]
    [InlineData("2: ", true, 2, null, null)]
    [InlineData("22: 33", true, 22, 33, null)]
    [InlineData("22: 33 : 99", true, 22, 33, 99)]
    [InlineData(": 33 : 99", true, null, 33, 99)]
    [InlineData(": 33 ", true, null, 33, null)]
    public void SliceSelectorTest(string test, bool r, int? start, int? end, int? step)
    {
        var p = JsonPathParser.SliceSelector;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
        {
            var s = v as SliceStatement;
            Assert.NotNull(s);
            Assert.Equal(start, s.Start);
            Assert.Equal(end, s.End);
            Assert.Equal(step, s.Step);
        }
    }

    [Theory]
    [InlineData("[-2]", true, -2)]
    [InlineData("[2]", true, 2)]
    [InlineData("[2", false, 0)]
    [InlineData("[2..]", false, 0)]
    public void IndexSegmentTest(string test, bool r, int rr)
    {
        var p = JsonPathParser.IndexSegment;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, (v as IndexSelectorStatment).Index);
    }

    [Theory]
    [InlineData("*", true)]
    [InlineData("[2]*", false)]
    [InlineData("[2", false)]
    [InlineData("[2..]", false)]
    public void WildcardSelectorTest(string test, bool r)
    {
        var p = JsonPathParser.WildcardSelector;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.IsType<WildcardSelectorStatment>(v);
    }

    [Theory]
    [InlineData("\"ss\\\"s\"", true, "ss\\\"s")]
    [InlineData("\"ss\r\ns\"", true, "ss\r\ns")]
    [InlineData("\"sss\"", true, "sss")]
    [InlineData("'sss'", true, "sss")]
    [InlineData("sss", false, "")]
    public void NameSelectorTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.NameSelector;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, (v as Member).Name);
    }

    [Theory]
    [InlineData("[\"ss\\\"s\"]", true, "ss\\\"s")]
    [InlineData("[\"ss\r\ns\"]", true, "ss\r\ns")]
    [InlineData("[\"sss\"]", true, "sss")]
    [InlineData("['sss']", true, "sss")]
    [InlineData("[sss]", false, "")]
    [InlineData(".sss", true, "sss")]
    [InlineData(".ss.s", true, "ss")]
    [InlineData("sss", false, "")]
    public void NameSegmentTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.NameSegment;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, (v as Member).Name);
    }

    [Theory]
    [InlineData("[\"ss\\\"s\"]", true, "[ss\\\"s]")]
    [InlineData("['s']['d']", true, "[s].[d]")]
    [InlineData("['s']['d']['d']['d']['d']['d']['d']", true, "[s].[d].[d].[d].[d].[d].[d]")]
    [InlineData("[1][3]", true, "[1].[3]")]
    [InlineData("[1]['s'][3]", true, "[1].[s].[3]")]
    [InlineData("[s]['d']", true, null)]
    [InlineData("[+99]['d']", true, null)]
    public void SingularQuerySegmentsTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.SingularQuerySegments;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, ToTestString(v));
    }

    [Theory]
    [InlineData("@[\"ss\\\"s\"]", true, "@.[ss\\\"s]")]
    [InlineData("@['s']['d']", true, "@.[s].[d]")]
    [InlineData("@['s']['d']['d']['d']['d']['d']['d']", true, "@.[s].[d].[d].[d].[d].[d].[d]")]
    [InlineData("@[1][3]", true, "@.[1].[3]")]
    [InlineData("@[1]['s'][3]", true, "@.[1].[s].[3]")]
    [InlineData("@[s]['d']", false, null)]
    [InlineData("@[+99]['d']", false, null)]
    [InlineData("[+99]['d']", false, null)]
    public void RelSingularQueryTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.RelSingularQuery.Eof();
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, ToTestString(v));
    }

    [Theory]
    [InlineData("$[\"ss\\\"s\"]", true, "$.[ss\\\"s]")]
    [InlineData("$['s']['d']", true, "$.[s].[d]")]
    [InlineData("$['s']['d']['d']['d']['d']['d']['d']", true, "$.[s].[d].[d].[d].[d].[d].[d]")]
    [InlineData("$[1][3]", true, "$.[1].[3]")]
    [InlineData("$[1]['s'][3]", true, "$.[1].[s].[3]")]
    [InlineData("$[s]['d']", false, null)]
    [InlineData("$[+99]['d']", false, null)]
    [InlineData("[+99]['d']", false, null)]
    public void AbsSingularQueryTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.AbsSingularQuery.Eof();
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, ToTestString(v));
    }

    [Theory]
    [InlineData("@[\"ss\\\"s\"]", true, "@.[ss\\\"s]")]
    [InlineData("@['s']['d']", true, "@.[s].[d]")]
    [InlineData("@['s']['d']['d']['d']['d']['d']['d']", true, "@.[s].[d].[d].[d].[d].[d].[d]")]
    [InlineData("@[1][3]", true, "@.[1].[3]")]
    [InlineData("@[1]['s'][3]", true, "@.[1].[s].[3]")]
    [InlineData("@[s]['d']", false, null)]
    [InlineData("@[+99]['d']", false, null)]
    [InlineData("[+99]['d']", false, null)]
    [InlineData("$[\"ss\\\"s\"]", true, "$.[ss\\\"s]")]
    [InlineData("$['s']['d']", true, "$.[s].[d]")]
    [InlineData("$['s']['d']['d']['d']['d']['d']['d']", true, "$.[s].[d].[d].[d].[d].[d].[d]")]
    [InlineData("$[1][3]", true, "$.[1].[3]")]
    [InlineData("$[1]['s'][3]", true, "$.[1].[s].[3]")]
    [InlineData("$[s]['d']", false, null)]
    [InlineData("$[+99]['d']", false, null)]
    public void SingularQueryTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.SingularQuery.Eof();
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, ToTestString(v));
    }

    [Theory]
    [InlineData("$", true, "$")]
    [InlineData("$$", false, "")]
    [InlineData("@", false, "")]
    [InlineData("$[\"ss\\\"s\"]", true, "$.[ss\\\"s]")]
    public void JsonPathParsersTest(string test, bool r, string rr)
    {
        var p = JsonPathParser.Parser;
        var b = p.TryParse(test, out var v, out var err);
        Assert.Equal(r, b);
        if (r)
            Assert.Equal(rr, ToTestString(v));
    }

    public static string ToTestString(IStatement statement)
    {
        if (statement is null)
            return null;
        return statement.ToString();
        //else if (statement is Member m)
        //    return m.Name;
        //else if (statement is IndexSelectorStatment i)
        //    return i.Index.ToString();
        //else if (statement is WildcardSelectorStatment)
        //    return "*";
        //else if (statement is SliceStatement s)
        //{
        //    return $"{(s.Start.HasValue ? s.Start.Value.ToString() : "")}:{(s.End.HasValue ? s.End.Value.ToString() : "")}{(s.Step.HasValue ? " :" + s.Step.Value.ToString() : "")}";
        //}
        //else if (statement is FunctionStatement f)
        //{
        //    return $"{f.Name}({string.Join(",", f.Arguments.Select(ToTestString))})";
        //}
        //else if (statement is OperatorStatement o)
        //{
        //    return $"{ToTestString(o.Left)} {o.Operator} {ToTestString(o.Right)}";
        //}
        //else if (statement is AndStatement and)
        //{
        //    return $"({ToTestString(and.Left)} && {ToTestString(and.Right)})";
        //}
        //else if (statement is OrStatement or)
        //{
        //    return $"({ToTestString(or.Left)} || {ToTestString(or.Right)})";
        //}
        //else if (statement is CurrentNode node)
        //{
        //    return "@" + (node.Child is not null ? "." + ToTestString(node.Child) : "");
        //}
        //else if (statement is RootNode rn)
        //{
        //    return "$" + (rn.Child is not null ? "." + ToTestString(rn.Child) : "");
        //}
        //else if (statement is UnionSelectionStatement u)
        //{
        //    return $"[{string.Join(",", u.List.Select(ToTestString))}]";
        //}
        //else if (statement is IStatementValue v)
        //{
        //    return (v.Value ?? "null").ToString();
        //}
        //else if (statement is UnaryOperaterStatement uo)
        //{
        //    if (uo.Operator.Equals("("))
        //    {
        //        return "(" + ToTestString(uo.Statement) + ")";
        //    }
        //    else if (uo.Operator.Equals("!"))
        //    {
        //        return "!(" + ToTestString(uo.Statement) + ")";
        //    }
        //    else
        //    {
        //        throw new NotSupportedException($"Not supported UnaryOperaterStatement Operator: {uo.Operator}");
        //    }
        //}
        //else if (statement is LinkNode ln)
        //{
        //    var sb = new System.Text.StringBuilder();
        //    IStatement cc = ln;
        //    while (cc is not null)
        //    {
        //        if (cc is LinkNode c)
        //        {
        //            sb.Append(ToTestString(c.Current));
        //            cc = c.Child;
        //            if (cc is not null)
        //                sb.Append(".");
        //        }
        //        else
        //        {
        //            sb.Append(ToTestString(cc));
        //            cc = null;
        //        }
        //    }
        //    return sb.ToString();
        //}
        //else
        //    throw new NotSupportedException($"Not supported statement type: {statement.GetType().FullName}");
    }
}