using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public class RequestPathHttpContextFieldConvertor : HttpContextFieldConvertor
{
    private readonly IHttpConditionStatement isnull;

    public RequestPathHttpContextFieldConvertor()
    {
        isnull = CreateAction(c => c.Request.Path.Value is null);
    }

    public override IHttpConditionStatement ConvertEqual(IStatement statement)
    {
        if (DefaultTemplateEngineFactory.TryGetString(statement, out var str))
        {
            return CreateAction(c => str == c.Request.Path.Value);
        }
        else if (statement is NullValueStatement)
            return isnull;
        else if (DefaultTemplateEngineFactory.TryGetStringFunc(statement, out var f))
            return CreateAction(c => f(c) == c.Request.Path.Value);
        else
            return null;
    }

    public override string Key()
    {
        return "field_Request.Path";
    }

    public override bool TryConvertBoolFunc(IStatement statement, out Func<HttpContext, bool> func)
    {
        func = AlwaysFalseFunc;
        return true;
    }

    public override bool TryConvertStringFunc(IStatement statement, out Func<HttpContext, string> func)
    {
        func = static c => c.Request.Path.Value;
        return true;
    }

    public override IStatement ConvertFieldStatement(FieldStatement field)
    {
        return new HttpTemplateFuncFieldStatement(field.Names, c => c.Request.Path.Value);
    }
}