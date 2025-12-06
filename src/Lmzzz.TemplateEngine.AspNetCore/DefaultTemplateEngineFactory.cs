using Lmzzz.Chars;
using Lmzzz.Chars.Fluent;
using Lmzzz.Template;
using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Lmzzz.AspNetCoreTemplate;

public class DefaultTemplateEngineFactory : ITemplateEngineFactory
{
    static DefaultTemplateEngineFactory()
    {
        AddFieldConvertor(new RequestPathHttpContextFieldConvertor());
        AddFieldConvertor(new RequestHostHttpContextFieldConvertor());
        AddFieldConvertor(new RequestContentTypeHttpContextFieldConvertor());
        AddFieldConvertor(new RequestProtocolHttpContextFieldConvertor());
        AddFieldConvertor(new RequestQueryStringHttpContextFieldConvertor());
        AddFieldConvertor(new RequestSchemeHttpContextFieldConvertor());
        AddFieldConvertor(new RequestMethodHttpContextFieldConvertor());
        AddFieldConvertor(new RequestHasFormContentTypeHttpContextFieldConvertor());
        AddFieldConvertor(new RequestIsHttpsHttpContextFieldConvertor());
        AddFieldConvertor(new RequestContentLengthHttpContextFieldConvertor());
        AddFieldConvertor(new RequestHeadersGenericHttpContextFieldConvertor());
        AddFieldConvertor(new RequestQueryGenericHttpContextFieldConvertor());
        AddFieldConvertor(new RequestCookiesGenericHttpContextFieldConvertor());
        AddFieldConvertor(new RequestFormGenericHttpContextFieldConvertor());
        AddFieldConvertor(new TraceIdentifierHttpContextFieldConvertor());
        AddFieldConvertor(new ResponseContentLengthHttpContextFieldConvertor());
        AddFieldConvertor(new ResponseContentTypeHttpContextFieldConvertor());
        AddFieldConvertor(new ResponseHasStartedHttpContextFieldConvertor());
        AddFieldConvertor(new ResponseStatusCodeHttpContextFieldConvertor());
        AddFieldConvertor(new ResponseHeadersGenericHttpContextFieldConvertor());
        TemplateEngine.SetOptimizer(c => OptimizeTemplateEngine(c, null));
    }

    public Func<HttpContext, bool> ConvertRouteFunction(string statement)
    {
        var localResult = new ParseResult<IConditionStatement>();
        var context = new CharParseContext(new StringCursor(statement));
        var success = TemplateEngineParser.ConditionParser.Parse(context, ref localResult);

        if (success)
        {
            var s = localResult.Value;
            return s is IHttpConditionStatement v ? v.EvaluateHttp : c => s.EvaluateCondition(new TemplateContext(c));
        }
        else
            throw new ParseException("Failed", context.Cursor.Position);
    }

    public Func<HttpContext, string> ConvertTemplate(string template)
    {
        var s = TemplateEngine.ToTemplate(template);
        if (s is IHttpTemplateStatement t)
        {
            return c =>
            {
                var sb = TemplateEngine.Pool.Get();
                try
                {
                    t.EvaluateHttpTemplate(c, sb);
                    return sb.ToString();
                }
                finally
                {
                    TemplateEngine.Pool.Return(sb);
                }
            };
        }
        return c => s.Evaluate(c);
    }

    public static IStatement OptimizeTemplateEngine(IStatement statement, Dictionary<string, HttpTemplateFuncFieldStatement> fields)
    {
        var r = statement;
        if (statement is EqualStatement equalStatement)
        {
            r = OptimizeEqualStatement(equalStatement, fields);
        }
        else if (statement is NotEqualStatement notEqualStatement)
        {
            var s = OptimizeEqualStatement(new EqualStatement(notEqualStatement.Left, notEqualStatement.Right), fields);
            if (s is IHttpConditionStatement c)
            {
                r = new ActionConditionStatement(cc => !c.EvaluateHttp(cc));
            }
        }
        else if (statement is FunctionStatement fs)
        {
            r = OptimizeFuncStatement(fs);
        }
        else if (statement is NotStatement sn)
        {
            if (sn.Statement is ActionConditionStatement actionCondition)
            {
                var ss = actionCondition.action;
                return new ActionConditionStatement(c => !ss(c));
            }
        }
        else if (statement is AndStatement asn)
        {
            if (asn.Left is ActionConditionStatement actionCondition && asn.Right is ActionConditionStatement actionConditionr)
            {
                var ss = actionCondition.action;
                var ssr = actionConditionr.action;
                return new ActionConditionStatement(c => ss(c) && ssr(c));
            }
        }
        else if (statement is OrStatement aso)
        {
            if (aso.Left is ActionConditionStatement actionCondition && aso.Right is ActionConditionStatement actionConditionr)
            {
                var ss = actionCondition.action;
                var ssr = actionConditionr.action;
                return new ActionConditionStatement(c => ss(c) || ssr(c));
            }
        }
        else if (statement == BoolValueStatement.False)
        {
            return HttpContextFieldConvertor.AlwaysFalse;
        }
        else if (statement == BoolValueStatement.True)
        {
            return HttpContextFieldConvertor.AlwaysTrue;
        }
        else if (statement is FieldStatement field)
        {
            if (IsIndexField(field, fields, out var f))
                r = f;
            else
                r = OptimizeFieldStatement(field);
        }
        else if (statement is OriginStrStatement originStrStatement)
        {
            return new HttpTemplateOriginStrStatement(originStrStatement.Text);
        }
        else if (statement is ReplaceStatement replaceStatement)
        {
            r = OptimizeHttpTemplateReplaceStatement(replaceStatement, fields);
        }
        else if (statement is ArrayStatement arrayStatement)
        {
            var a = arrayStatement.Statements.Select(i => OptimizeTemplateEngine(i, fields) as IHttpTemplateStatement).ToArray();
            if (a.All(static i => i is not null))
            {
                r = new HttpTemplateArrayStatement(a);
            }
        }
        else if (statement is IfConditionStatement ifStatement)
        {
            var s = OptimizeTemplateEngine(ifStatement.Condition, fields) as IHttpConditionStatement;
            if (s != null)
            {
                var t = OptimizeTemplateEngine(ifStatement.Text, fields) as IHttpTemplateStatement;
                if (t != null)
                    r = new HttpTemplateIfConditionStatement(s, t);
            }
        }
        else if (statement is IfStatement ifs)
        {
            var s = OptimizeTemplateEngine(ifs.If, fields) as IHttpIfConditionStatement;
            if (s != null)
            {
                var d = new HttpTemplateIfStatement()
                {
                    If = ifs.If,
                    Else = ifs.Else,
                    ElseIfs = ifs.ElseIfs,
                    IfHttp = s
                };
                if (ifs.Else != null)
                {
                    var es = OptimizeTemplateEngine(ifs.Else, fields) as IHttpTemplateStatement;
                    if (es != null)
                    {
                        d.ElseHttp = es;
                    }
                    else
                        return r ?? statement;
                }

                if (ifs.ElseIfs != null)
                {
                    var el = ifs.ElseIfs.Select(i => OptimizeTemplateEngine(i, fields) as IHttpIfConditionStatement).ToArray();
                    if (el.Any(i => i == null))
                        return r ?? statement;
                    d.ElseIfsHttp = el;
                }
                r = d;
            }
        }
        else if (statement is ForStatement fors)
        {
            fields ??= new Dictionary<string, HttpTemplateFuncFieldStatement>();
            var itemName = fors.ItemName;
            var indexName = fors.IndexName;
            if (itemName != null)
            {
                var v = new HttpTemplateFuncFieldStatement([itemName], c => c.Items[itemName]);
                fields[itemName] = v;
            }
            if (indexName != null)
            {
                var v = new HttpTemplateFuncFieldStatement([indexName], c => c.Items[indexName]);
                fields[indexName] = v;
            }
            var s = OptimizeTemplateEngine(fors.Value, fields) as IObjectHttpStatement;
            if (s != null)
            {
                var t = OptimizeTemplateEngine(fors.Statement, fields) as IHttpTemplateStatement;
                if (t != null)
                {
                    r = new HttpTemplateForStatement()
                    {
                        IndexName = fors.IndexName,
                        Value = fors.Value,
                        ItemName = fors.ItemName,
                        Statement = fors.Statement,
                        StatementHttp = t,
                        ValueHttp = s
                    };
                    // r.Visit(ss => ChangeFields(ss, v, itemName, i, indexName));
                }
            }
        }

        return r ?? statement;
    }

    //private static void ChangeFields(IStatement ss, HttpTemplateFuncFieldStatement? v, string? itemName, HttpTemplateFuncFieldStatement? i, string? indexName)
    //{
    //    if (IsIndexField(ss, itemName))
    //    {
    //    }
    //    else if (IsIndexField(ss, indexName))
    //    {
    //    }
    //}

    //private static bool IsIndexField(IStatement ss, string? itemName)
    //{
    //    return ss is FieldStatement field && field.Names.Count == 1 && field.Names[0].Equals(itemName, StringComparison.OrdinalIgnoreCase);
    //}

    private static bool IsIndexField(IStatement ss, Dictionary<string, HttpTemplateFuncFieldStatement> dict, out HttpTemplateFuncFieldStatement f)
    {
        f = null;
        return ss is FieldStatement field && dict is not null && field.Names.Count == 1 && dict.TryGetValue(field.Key, out f);
    }

    private static IStatement OptimizeFieldStatement(FieldStatement field)
    {
        if (fieldConvertor.TryGetValue(field.Key, out var c))
        {
            var r = c.ConvertFieldStatement(field);
            if (r != null)
                return r;
        }
        foreach (var item in GenericConvertors)
        {
            if (field.Key.StartsWith(item.Key(), StringComparison.OrdinalIgnoreCase))
            {
                var r = item.ConvertFieldStatement(field);
                if (r != null)
                    return r;
            }
        }
        return null;
    }

    private static IStatement OptimizeHttpTemplateReplaceStatement(ReplaceStatement replaceStatement, Dictionary<string, HttpTemplateFuncFieldStatement> fields)
    {
        if (replaceStatement.Statement is StringValueStatement stringValueStatement)
        {
            return new HttpTemplateOriginStrStatement(stringValueStatement.Value);
        }
        else if (replaceStatement.Statement is DecimalValueStatement decimalValueStatement)
        {
            return new HttpTemplateOriginStrStatement(decimalValueStatement.Value.ToString());
        }
        else if (replaceStatement.Statement is BoolValueStatement boolValueStatement)
        {
            return new HttpTemplateOriginStrStatement(boolValueStatement.Value.ToString());
        }
        else if (replaceStatement.Statement is NullValueStatement nullValueStatement)
        {
            return new HttpTemplateOriginStrStatement(string.Empty);
        }
        else if (replaceStatement.Statement is FieldStatement fieldStatement)
        {
            if (IsIndexField(fieldStatement, fields, out var f))
                return new HttpTemplateReplaceStatementFunc(replaceStatement.Statement, c => f.EvaluateObjectHttp(c)?.ToString());

            if (fieldConvertor.TryGetValue(fieldStatement.Key, out var c))
            {
                if (c.TryConvertStringFunc(fieldStatement, out var func))
                    return new HttpTemplateReplaceStatementFunc(replaceStatement.Statement, func);
                var r = c.ConvertFieldStatement(fieldStatement);
                if (r != null && r is IHttpTemplateStatement)
                    return r;
            }

            foreach (var item in GenericConvertors)
            {
                if (fieldStatement.Key.StartsWith(item.Key(), StringComparison.OrdinalIgnoreCase))
                {
                    if (item.TryConvertStringFunc(fieldStatement, out var func))
                        return new HttpTemplateReplaceStatementFunc(replaceStatement.Statement, func);
                    var r = item.ConvertFieldStatement(fieldStatement);
                    if (r != null && r is IHttpTemplateStatement)
                        return r;
                }
            }
        }
        else if (replaceStatement is IHttpTemplateStatement)
        {
            return replaceStatement;
        }
        else if (replaceStatement.Statement is IHttpTemplateStatement)
        {
            return replaceStatement.Statement;
        }

        return null;
    }

    private static IStatement OptimizeFuncStatement(FunctionStatement fs)
    {
        if (funcConvertor.TryGetValue(fs.Name, out var func))
            return func(fs);
        return null;
    }

    private static IHttpConditionStatement OptimizeEqualStatement(EqualStatement equalStatement, Dictionary<string, HttpTemplateFuncFieldStatement> fields)
    {
        if (equalStatement.Left is FieldStatement f)
        {
            if (fieldConvertor.TryGetValue(f.Key, out var c))
                return c.ConvertEqual(equalStatement.Right);

            foreach (var item in GenericConvertors)
            {
                if (f.Key.StartsWith(item.Key(), StringComparison.OrdinalIgnoreCase))
                {
                    return item.GenericConvertEqual(f, equalStatement.Right, fields);
                }
            }
        }

        if (equalStatement.Right is FieldStatement fr)
        {
            if (fieldConvertor.TryGetValue(fr.Key, out var c))
                return c.ConvertEqual(equalStatement.Left);

            foreach (var item in GenericConvertors)
            {
                if (fr.Key.StartsWith(item.Key(), StringComparison.OrdinalIgnoreCase))
                {
                    return item.GenericConvertEqual(fr, equalStatement.Left, fields);
                }
            }
        }
        return null;
    }

    public static bool TryGetStringFunc(IStatement statement, out Func<HttpContext, string> func)
    {
        if (statement is FieldStatement f)
        {
            if (fieldConvertor.TryGetValue(f.Key, out var c) && c.TryConvertStringFunc(statement, out func))
                return true;

            foreach (var item in GenericConvertors)
            {
                if (f.Key.StartsWith(item.Key(), StringComparison.OrdinalIgnoreCase))
                {
                    if (item.TryConvertStringFunc(statement, out func))
                        return true;
                    break;
                }
            }
        }

        if (statement is IHttpConditionStatement s)
        {
            func = c => s.EvaluateHttp(c) ? true.ToString() : false.ToString();
            return true;
        }

        func = null;
        return false;
    }

    public static bool TryGetBoolFunc(IStatement statement, out Func<HttpContext, bool> func)
    {
        if (statement is FieldStatement f)
        {
            if (fieldConvertor.TryGetValue(f.Key, out var c) && c.TryConvertBoolFunc(statement, out func))
                return true;

            foreach (var item in GenericConvertors)
            {
                if (f.Key.StartsWith(item.Key(), StringComparison.OrdinalIgnoreCase))
                {
                    if (item.TryConvertBoolFunc(statement, out func))
                        return true;
                    break;
                }
            }
        }

        if (statement is IHttpConditionStatement s)
        {
            func = c => s.EvaluateHttp(c) ? true : false;
            return true;
        }

        func = null;
        return false;
    }

    public static bool TryGetBool(IStatement statement, out bool b)
    {
        if (statement is BoolValueStatement bb)
        {
            b = bb.Value;
            return true;
        }
        else if (statement == HttpContextFieldConvertor.AlwaysFalse)
        {
            b = false;
            return true;
        }
        else if (statement == HttpContextFieldConvertor.AlwaysTrue)
        {
            b = true;
            return true;
        }

        b = false;
        return false;
    }

    public static bool TryGetDecimal(IStatement statement, out decimal d)
    {
        if (statement is DecimalValueStatement dd)
        {
            d = dd.Value;
            return true;
        }
        else if (statement is StringValueStatement s)
        {
            return Decimal.TryParse(s.Value, out d);
        }

        d = 0;
        return false;
    }

    public static bool TryGetString(IStatement statement, out string str)
    {
        if (statement is StringValueStatement s)
        {
            str = s.Value;
            return true;
        }
        else if (statement is BoolValueStatement b)
        {
            str = b.Value.ToString();
            return true;
        }
        else if (statement is DecimalValueStatement d)
        {
            str = d.Value.ToString();
            return true;
        }
        else if (statement == HttpContextFieldConvertor.AlwaysFalse)
        {
            str = false.ToString();
            return true;
        }
        else if (statement == HttpContextFieldConvertor.AlwaysTrue)
        {
            str = true.ToString();
            return true;
        }

        str = null;
        return false;
    }

    public static void AddFieldConvertor(HttpContextFieldConvertor c)
    {
        if (c is GenericHttpContextFieldConvertor g)
            GenericConvertors.Add(g);
        else
            fieldConvertor[c.Key()] = c;
    }

    private static readonly List<GenericHttpContextFieldConvertor> GenericConvertors = new List<GenericHttpContextFieldConvertor>();

    private static readonly Dictionary<string, HttpContextFieldConvertor> fieldConvertor = new Dictionary<string, HttpContextFieldConvertor>(StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, Func<FunctionStatement, IStatement>> funcConvertor = new Dictionary<string, Func<FunctionStatement, IStatement>>(StringComparer.OrdinalIgnoreCase)
    {
        { "Regex", fs=>
        {
            var o =RegexOptions.Compiled;
            if(fs.Arguments.Length >= 3)
            {
                if(fs.Arguments[2] is StringValueStatement v)
                {
                    if(v.Value != null && Enum.TryParse<RegexOptions>(v.Value , true, out var r))
                    {
                        o = r;
                    }
                }
                else if(fs.Arguments[2] is DecimalValueStatement d)
                {
                    o = (RegexOptions)Convert.ToInt32(d.Value);
                }
            }
            if (fs.Arguments[0] is StringValueStatement s && fs.Arguments[1] is StringValueStatement svv )
            {
                var reg = new Regex(svv.Value, o);
                return new BoolConditionStatement(reg.IsMatch(s.Value));
            }
            else if (TryGetStringFunc(fs.Arguments[0], out var func) && fs.Arguments[1] is StringValueStatement sv)
            {
                var reg = new Regex(sv.Value, o);
                return new ActionConditionStatement(c => reg.IsMatch(func(c)));
            }

           return null;
        } },
        { "In", fs =>
        {
            if(fs.Arguments.Length <= 1) return HttpContextFieldConvertor.AlwaysFalse;

            if (TryGetStringFunc(fs.Arguments[0], out var func))
            {
                if(fs.Arguments.Skip(1).All(static i => i is StringValueStatement))
                {
                    var a = fs.Arguments.Skip(1).Select(static i => (i as StringValueStatement).Value).Distinct().ToFrozenSet();
                    return new ActionConditionStatement(c => a.Contains(func(c)));
                }
            }
            else if(TryGetBoolFunc(fs.Arguments[0], out var f))
            {
                if(fs.Arguments.Skip(1).All(static i => i is BoolValueStatement))
                {
                    var a = fs.Arguments.Skip(1).Select(static i => (i as BoolValueStatement).Value).Distinct().ToArray();
                    return new ActionConditionStatement(c => a.Contains(f(c)));
                }
            }

            return null;
        } },
        { "InIgnoreCase", fs =>
        {
            if(fs.Arguments.Length <= 1) return HttpContextFieldConvertor.AlwaysFalse;

            if (TryGetStringFunc(fs.Arguments[0], out var func))
            {
                if(fs.Arguments.Skip(1).All(static i => i is StringValueStatement))
                {
                    var a = fs.Arguments.Skip(1).Select(static i => (i as StringValueStatement).Value).Distinct(StringComparer.OrdinalIgnoreCase).ToFrozenSet(StringComparer.OrdinalIgnoreCase);
                    return new ActionConditionStatement(c => a.Contains(func(c)));
                }
            }
            else if(TryGetBoolFunc(fs.Arguments[0], out var f))
            {
                if(fs.Arguments.Skip(1).All(static i => i is BoolValueStatement))
                {
                    var a = fs.Arguments.Skip(1).Select(static i => (i as BoolValueStatement).Value).Distinct().ToArray();
                    return new ActionConditionStatement(c => a.Contains(f(c)));
                }
            }

            return null;
        } },
        { "EqualIgnoreCase", fs =>
            {
                if(fs.Arguments.Length <= 1) return HttpContextFieldConvertor.AlwaysFalse;
                if(DefaultTemplateEngineFactory.TryGetString(fs.Arguments[0], out var s))
                {
                    if(DefaultTemplateEngineFactory.TryGetString(fs.Arguments[1], out var r))
                    {
                        return new BoolConditionStatement(string.Equals(s, r, StringComparison.OrdinalIgnoreCase));
                    }

                    if(DefaultTemplateEngineFactory.TryGetStringFunc(fs.Arguments[1], out var fr))
                    {
                        return new ActionConditionStatement(c => string.Equals(s, fr(c), StringComparison.OrdinalIgnoreCase));
                    }
                }

                if(DefaultTemplateEngineFactory.TryGetStringFunc(fs.Arguments[0], out var ls))
                {
                    if(DefaultTemplateEngineFactory.TryGetString(fs.Arguments[1], out var r))
                    {
                        return new ActionConditionStatement(c => string.Equals(ls(c), r, StringComparison.OrdinalIgnoreCase));
                    }

                    if(DefaultTemplateEngineFactory.TryGetStringFunc(fs.Arguments[1], out var fr))
                    {
                        return new ActionConditionStatement(c => string.Equals(ls(c), fr(c), StringComparison.OrdinalIgnoreCase));
                    }
                }

                return null;
            } }
    };
}