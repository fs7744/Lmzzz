using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public class RequestFormGenericHttpContextFieldConvertor : GenericHttpContextFieldConvertor
{
    public override string Key()
    {
        return "field_Request.Form";
    }

    public override bool TryConvertStringFunc(IStatement statement, out Func<HttpContext, string> func)
    {
        if (statement is FieldStatement field)
        {
            if (field.Names.Count == 2)
            {
                func = c => c.Request.Form.ToString();
                return true;
            }
            else if (field.Names.Count >= 3)
            {
                var k = field.Names[2];
                if (field.Names.Count == 3)
                {
                    func = c => c.Request.Form[k].ToString();
                    return true;
                }
                else if (field.Names.Count == 4 && int.TryParse(field.Names.Last(), out var i) && i >= 0)
                {
                    func = c =>
                    {
                        var o = c.Request.Form[k];
                        if (i < o.Count)
                            return o[i];
                        else
                            return null;
                    };
                    return true;
                }
                else if (field.Names.Count == 4 && "Count".Equals(field.Names.Last(), StringComparison.OrdinalIgnoreCase))
                {
                    func = c => c.Request.Form[k].Count.ToString();
                    return true;
                }
                else
                {
                    var f = FieldStatement.CreateGetter(field.Names.Skip(3), Template.FieldStatementMode.Defined);
                    func = c => f(c.Request.Form[k])?.ToString();
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
            return new HttpTemplateFuncFieldStatement(field.Names, static c => c.Request.Form);
        }
        else if (field.Names.Count >= 3)
        {
            var k = field.Names[2];
            if (field.Names.Count == 3)
            {
                return new HttpTemplateFuncFieldStatement(field.Names, c => c.Request.Form[k]);
            }
            else if (field.Names.Count == 4 && int.TryParse(field.Names.Last(), out var i) && i >= 0)
            {
                return new HttpTemplateFuncFieldStatement(field.Names, c =>
                {
                    var o = c.Request.Form[k];
                    if (i < o.Count)
                        return o[i];
                    else
                        return null;
                });
            }
            else if (field.Names.Count == 4 && "Count".Equals(field.Names.Last(), StringComparison.OrdinalIgnoreCase))
            {
                return new HttpTemplateFuncFieldStatement(field.Names, c => c.Request.Form[k].Count);
            }
            else
            {
                var f = FieldStatement.CreateGetter(field.Names.Skip(3), Template.FieldStatementMode.Defined);
                return new HttpTemplateFuncFieldStatement(field.Names, c => f(c.Request.Form[k]));
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
                return new ActionConditionStatement(c => EqualStatement.Eqs(c.Request.Form, o.EvaluateObjectHttp(c)));
        }
        else if (field.Names.Count >= 3)
        {
            var k = field.Names[2];
            if (field.Names.Count == 3)
            {
                if (DefaultTemplateEngineFactory.TryGetString(statement, out var fs))
                {
                    return new ActionConditionStatement(c => c.Request.Form[k].ToString() == fs);
                }

                if (DefaultTemplateEngineFactory.TryGetStringFunc(statement, out var f))
                {
                    return new ActionConditionStatement(c => c.Request.Form[k].ToString() == f(c));
                }

                var s = DefaultTemplateEngineFactory.OptimizeTemplateEngine(statement, fields);
                if (s is IObjectHttpStatement o)
                    return new ActionConditionStatement(c => EqualStatement.Eqs(c.Request.Form[k].ToString(), o.EvaluateObjectHttp(c)));
            }
            else if (field.Names.Count == 4 && int.TryParse(field.Names.Last(), out var i) && i >= 0)
            {
                if (DefaultTemplateEngineFactory.TryGetString(statement, out var fs))
                {
                    return new ActionConditionStatement(c =>
                    {
                        var o = c.Request.Form[k];
                        if (i < o.Count)
                            return o[i] == fs;
                        else
                            return null == fs;
                    });
                }
                if (DefaultTemplateEngineFactory.TryGetStringFunc(statement, out var f))
                {
                    return new ActionConditionStatement(c =>
                    {
                        var o = c.Request.Form[k];
                        if (i < o.Count)
                            return o[i] == f(c);
                        else
                            return null == f(c);
                    });
                }

                var s = DefaultTemplateEngineFactory.OptimizeTemplateEngine(statement, fields);
                if (s is IObjectHttpStatement of)
                    return new ActionConditionStatement(c =>
                    {
                        var o = c.Request.Form[k];
                        if (i < o.Count)
                            return EqualStatement.Eqs(o[i], of.EvaluateObjectHttp(c));
                        else
                            return null == of.EvaluateObjectHttp(c);
                    });
            }
            else if (field.Names.Count == 4 && "Count".Equals(field.Names.Last(), StringComparison.OrdinalIgnoreCase))
            {
                if (DefaultTemplateEngineFactory.TryGetDecimal(statement, out var d))
                {
                    return new ActionConditionStatement(c => c.Request.Form[k].Count == d);
                }
            }
            else
            {
                var s = DefaultTemplateEngineFactory.OptimizeTemplateEngine(statement, fields);
                if (s is IObjectHttpStatement o)
                {
                    var f = FieldStatement.CreateGetter(field.Names.Skip(3), Template.FieldStatementMode.Defined);
                    return new ActionConditionStatement(c => EqualStatement.Eqs(f(c.Request.Form[k]), o.EvaluateObjectHttp(c)));
                }
            }
        }

        return base.GenericConvertEqual(field, statement, fields);
    }
}