using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public class RequestCookiesGenericHttpContextFieldConvertor : GenericHttpContextFieldConvertor
{
    public override string Key()
    {
        return "field_Request.Cookies";
    }

    public override bool TryConvertStringFunc(IStatement statement, out Func<HttpContext, string> func)
    {
        if (statement is FieldStatement field)
        {
            if (field.Names.Count == 2)
            {
                func = c => c.Request.Cookies?.ToString();
                return true;
            }
            else if (field.Names.Count >= 3)
            {
                var k = field.Names[2];
                if (field.Names.Count == 3)
                {
                    func = c => c.Request.Cookies[k]?.ToString();
                    return true;
                }
                else if (field.Names.Count == 4 && "Length".Equals(field.Names.Last(), StringComparison.OrdinalIgnoreCase))
                {
                    func = c => c.Request.Cookies[k]?.Length.ToString();
                    return true;
                }
                else
                {
                    var f = FieldStatement.CreateGetter(field.Names.Skip(3), Template.FieldStatementMode.Defined);
                    func = c => f(c.Request.Cookies[k])?.ToString();
                    return true;
                }
            }
        }
        return base.TryConvertStringFunc(statement, out func);
    }

    public override IStatement ConvertFieldStatement(FieldStatement field)
    {
        if (field.Names.Count == 2)
        {
            return new HttpTemplateFuncFieldStatement(field.Names, static c => c.Request.Cookies);
        }
        else if (field.Names.Count >= 3)
        {
            var k = field.Names[2];
            if (field.Names.Count == 3)
            {
                return new HttpTemplateFuncFieldStatement(field.Names, c => c.Request.Cookies[k]);
            }
            else if (field.Names.Count == 4 && "Length".Equals(field.Names.Last(), StringComparison.OrdinalIgnoreCase))
            {
                return new HttpTemplateFuncFieldStatement(field.Names, c => c.Request.Cookies[k]?.Length);
            }
            else
            {
                var f = FieldStatement.CreateGetter(field.Names.Skip(3), Template.FieldStatementMode.Defined);
                return new HttpTemplateFuncFieldStatement(field.Names, c => f(c.Request.Cookies[k]));
            }
        }

        return base.ConvertFieldStatement(field);
    }

    public override IHttpConditionStatement GenericConvertEqual(FieldStatement field, IStatement statement, Dictionary<string, HttpTemplateFuncFieldStatement> fields)
    {
        if (field.Names.Count == 2)
        {
            var s = DefaultTemplateEngineFactory.OptimizeTemplateEngine(statement, fields);
            if (s is IObjectHttpStatement o)
                return new ActionConditionStatement(c => EqualStatement.Eqs(c.Request.Cookies, o.EvaluateObjectHttp(c)));
        }
        else if (field.Names.Count >= 3)
        {
            var k = field.Names[2];
            if (field.Names.Count == 3)
            {
                if (DefaultTemplateEngineFactory.TryGetString(statement, out var fs))
                {
                    return new ActionConditionStatement(c => c.Request.Cookies[k]?.ToString() == fs);
                }

                if (DefaultTemplateEngineFactory.TryGetStringFunc(statement, out var f))
                {
                    return new ActionConditionStatement(c => c.Request.Cookies[k]?.ToString() == f(c));
                }

                var s = DefaultTemplateEngineFactory.OptimizeTemplateEngine(statement, fields);
                if (s is IObjectHttpStatement o)
                    return new ActionConditionStatement(c => EqualStatement.Eqs(c.Request.Cookies[k].ToString(), o.EvaluateObjectHttp(c)));
            }
            else if (field.Names.Count == 4 && "Length".Equals(field.Names.Last(), StringComparison.OrdinalIgnoreCase))
            {
                if (DefaultTemplateEngineFactory.TryGetDecimal(statement, out var d))
                {
                    return new ActionConditionStatement(c => c.Request.Cookies[k]?.Length == d);
                }
            }
            else
            {
                var s = DefaultTemplateEngineFactory.OptimizeTemplateEngine(statement, fields);
                if (s is IObjectHttpStatement o)
                {
                    var f = FieldStatement.CreateGetter(field.Names.Skip(3), Template.FieldStatementMode.Defined);
                    return new ActionConditionStatement(c => EqualStatement.Eqs(f(c.Request.Cookies[k]), o.EvaluateObjectHttp(c)));
                }
            }
        }

        return base.GenericConvertEqual(field, statement, fields);
    }
}