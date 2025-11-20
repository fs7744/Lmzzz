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
        AddFieldConvertor(new TraceIdentifierHttpContextFieldConvertor());
        AddFieldConvertor(new ResponseContentLengthHttpContextFieldConvertor());
        AddFieldConvertor(new ResponseContentTypeHttpContextFieldConvertor());
        AddFieldConvertor(new ResponseHasStartedHttpContextFieldConvertor());
        AddFieldConvertor(new ResponseStatusCodeHttpContextFieldConvertor());
        TemplateEngine.SetOptimizer(OptimizeTemplateEngine);
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

    public static IStatement OptimizeTemplateEngine(IStatement statement)
    {
        var r = statement;
        if (statement is EqualStatement equalStatement)
        {
            r = OptimizeEqualStatement(equalStatement);
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
            r = OptimizeFieldStatement(field);
        }
        else if (statement is OriginStrStatement originStrStatement)
        {
            return new HttpTemplateOriginStrStatement(originStrStatement.Text);
        }
        else if (statement is ReplaceStatement replaceStatement)
        {
            r = OptimizeHttpTemplateReplaceStatement(replaceStatement);
        }
        else if (statement is ArrayStatement arrayStatement)
        {
            if (arrayStatement.Statements.All(static i => i is IHttpTemplateStatement))
            {
                r = new HttpTemplateArrayStatement(arrayStatement.Statements.Select(static i => i as IHttpTemplateStatement).ToArray());
            }
        }

        return r ?? statement;
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

    private static IStatement OptimizeHttpTemplateReplaceStatement(ReplaceStatement replaceStatement)
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

    private static IHttpConditionStatement OptimizeEqualStatement(EqualStatement equalStatement)
    {
        if (equalStatement.Left is FieldStatement f)
        {
            if (fieldConvertor.TryGetValue(f.Key, out var c))
                return c.ConvertEqual(equalStatement.Right);

            foreach (var item in GenericConvertors)
            {
                if (f.Key.StartsWith(item.Key(), StringComparison.OrdinalIgnoreCase))
                {
                    return item.GenericConvertEqual(f, equalStatement.Right);
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
                    return item.GenericConvertEqual(fr, equalStatement.Left);
                }
            }
        }
        return null;
    }

    public static bool TryGetStringFunc(IStatement statement, out Func<HttpContext, string> func)
    {
        if (statement is FieldStatement f && fieldConvertor.TryGetValue(f.Key, out var c) && c.TryConvertStringFunc(statement, out func))
        {
            return true;
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
        if (statement is FieldStatement f && fieldConvertor.TryGetValue(f.Key, out var c) && c.TryConvertBoolFunc(statement, out func))
        {
            return true;
        }

        if (statement is IHttpConditionStatement s)
        {
            func = c => s.EvaluateHttp(c) ? true : false;
            return true;
        }

        func = null;
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
            else if (fs.Arguments[0] is FieldStatement f && fs.Arguments[1] is StringValueStatement sv
                && fieldConvertor.TryGetValue(f.Key, out var c) && c.TryConvertStringFunc(null, out var func))
            {
                var reg = new Regex(sv.Value, o);
                return new ActionConditionStatement(c => reg.IsMatch(func(c)));
            }
            else
                return null;
        } },
        { "In", fs =>
        {
            if(fs.Arguments.Length <= 1) return HttpContextFieldConvertor.AlwaysFalse;

            if (fs.Arguments[0] is FieldStatement f
                && fieldConvertor.TryGetValue(f.Key, out var c))
            {
                if(fs.Arguments.Skip(1).All(static i => i is StringValueStatement))
                {
                    if(c.TryConvertStringFunc(null, out var func))
                    {
                       var a = fs.Arguments.Skip(1).Select(static i => (i as StringValueStatement).Value).Distinct().ToFrozenSet();
                        return new ActionConditionStatement(c => a.Contains(func(c)));
                    }
                }
                else if(fs.Arguments.Skip(1).All(static i => i is BoolValueStatement))
                {
                    if(c.TryConvertBoolFunc(null, out var func))
                    {
                        var a = fs.Arguments.Skip(1).Select(static i => (i as BoolValueStatement).Value).Distinct().ToArray();
                        return new ActionConditionStatement(c => a.Contains(func(c)));
                    }
                }
            }

            return null;
        } },
        { "InIgnoreCase", fs =>
        {
            if(fs.Arguments.Length <= 1) return HttpContextFieldConvertor.AlwaysFalse;

            if (fs.Arguments[0] is FieldStatement f
                && fieldConvertor.TryGetValue(f.Key, out var c))
            {
                if(fs.Arguments.Skip(1).All(static i => i is StringValueStatement))
                {
                    if(c.TryConvertStringFunc(null, out var func))
                    {
                       var a = fs.Arguments.Skip(1).Select(static i => (i as StringValueStatement).Value).Distinct(StringComparer.OrdinalIgnoreCase).ToFrozenSet(StringComparer.OrdinalIgnoreCase);
                        return new ActionConditionStatement(c => a.Contains(func(c)));
                    }
                }
                else if(fs.Arguments.Skip(1).All(static i => i is BoolValueStatement))
                {
                    if(c.TryConvertBoolFunc(null, out var func))
                    {
                        var a = fs.Arguments.Skip(1).Select(static i => (i as BoolValueStatement).Value).Distinct().ToArray();
                        return new ActionConditionStatement(c => a.Contains(func(c)));
                    }
                }
            }

            return null;
        } }
    };
}