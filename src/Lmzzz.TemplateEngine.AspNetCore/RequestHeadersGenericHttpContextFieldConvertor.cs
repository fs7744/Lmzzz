using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public class RequestHeadersGenericHttpContextFieldConvertor : GenericHttpContextFieldConvertor
{
    public override string Key()
    {
        return "field_Request.Headers";
    }

    public override bool TryConvertStringFunc(IStatement statement, out Func<HttpContext, string> func)
    {
        if (statement is FieldStatement field)
        {
            if (field.Names.Count == 2)
            {
                func = c => c.Request.Headers.ToString();
                return true;
            }
            else if (field.Names.Count >= 3)
            {
                var k = field.Names[2];
                if (field.Names.Count == 3)
                {
                    func = c => c.Request.Headers[k].ToString();
                    return true;
                }
                else if (field.Names.Count == 4 && int.TryParse(field.Names.Last(), out var i) && i >= 0)
                {
                    func = c =>
                    {
                        var o = c.Request.Headers[k];
                        if (i < o.Count)
                            return o[i];
                        else
                            return null;
                    };
                    return true;
                }
                else if (field.Names.Count == 4 && "Count".Equals(field.Names.Last(), StringComparison.OrdinalIgnoreCase))
                {
                    func = c => c.Request.Headers[k].Count.ToString();
                    return true;
                }
                else
                {
                    var f = FieldStatement.CreateGetter(field.Names.Skip(3), Template.FieldStatementMode.Defined);
                    func = c => f(c.Request.Headers[k])?.ToString();
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
            return new HttpTemplateFuncFieldStatement(field.Names, static c => c.Request.Headers);
        }
        else if (field.Names.Count >= 3)
        {
            var k = field.Names[2];
            if (field.Names.Count == 3)
            {
                return new HttpTemplateFuncFieldStatement(field.Names, c => c.Request.Headers[k]);
            }
            else if (field.Names.Count == 4 && int.TryParse(field.Names.Last(), out var i) && i >= 0)
            {
                return new HttpTemplateFuncFieldStatement(field.Names, c =>
                {
                    var o = c.Request.Headers[k];
                    if (i < o.Count)
                        return o[i];
                    else
                        return null;
                });
            }
            else if (field.Names.Count == 4 && "Count".Equals(field.Names.Last(), StringComparison.OrdinalIgnoreCase))
            {
                return new HttpTemplateFuncFieldStatement(field.Names, c => c.Request.Headers[k].Count);
            }
            else
            {
                var f = FieldStatement.CreateGetter(field.Names.Skip(3), Template.FieldStatementMode.Defined);
                return new HttpTemplateFuncFieldStatement(field.Names, c => f(c.Request.Headers[k]));
            }
        }

        return base.ConvertFieldStatement(field);
    }

    //public override IHttpConditionStatement GenericConvertEqual(FieldStatement field, IStatement statement)
    //{
    //    if (field.Names.Count == 2)
    //    {
    //        var s = DefaultTemplateEngineFactory.OptimizeTemplateEngine(statement);
    //        if (s is )
    //            return new HttpTemplateFuncFieldStatement(field.Names, static c => c.Request.Headers);
    //    }
    //    else if (field.Names.Count >= 3)
    //    {
    //        var k = field.Names[2];
    //        if (field.Names.Count == 3)
    //        {
    //            return new HttpTemplateFuncFieldStatement(field.Names, c => c.Request.Headers[k]);
    //        }
    //        else if (field.Names.Count == 4 && int.TryParse(field.Names.Last(), out var i) && i >= 0)
    //        {
    //            return new HttpTemplateFuncFieldStatement(field.Names, c =>
    //            {
    //                var o = c.Request.Headers[k];
    //                if (i < o.Count)
    //                    return o[i];
    //                else
    //                    return null;
    //            });
    //        }
    //        else if (field.Names.Count == 4 && "Count".Equals(field.Names.Last(), StringComparison.OrdinalIgnoreCase))
    //        {
    //            return new HttpTemplateFuncFieldStatement(field.Names, c => c.Request.Headers[k].Count);
    //        }
    //        else
    //        {
    //            var f = FieldStatement.CreateGetter(field.Names.Skip(3), Template.FieldStatementMode.Defined);
    //            return new HttpTemplateFuncFieldStatement(field.Names, c => f(c.Request.Headers[k]));
    //        }
    //    }

    //    return base.GenericConvertEqual(field, statement);
    //}
}