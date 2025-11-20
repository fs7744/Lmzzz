using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public class ResponseContentTypeHttpContextFieldConvertor : HttpContextFieldConvertor
{
    private readonly IHttpConditionStatement isnull;

    public ResponseContentTypeHttpContextFieldConvertor()
    {
        isnull = CreateAction(c => c.Response.ContentType is null);
    }

    public override IHttpConditionStatement ConvertEqual(IStatement statement)
    {
        if (TryGetString(statement, out var str))
        {
            return CreateAction(c => str == c.Response.ContentType);
        }
        else if (statement is NullValueStatement)
            return isnull;
        else if (DefaultTemplateEngineFactory.TryGetStringFunc(statement, out var f))
            return CreateAction(c => f(c) == c.Response.ContentType);
        else
            return null;
    }

    public override string Key()
    {
        return "field_Response.ContentType";
    }

    public override bool TryConvertBoolFunc(IStatement statement, out Func<HttpContext, bool> func)
    {
        func = AlwaysFalseFunc;
        return true;
    }

    public override bool TryConvertStringFunc(IStatement statement, out Func<HttpContext, string> func)
    {
        func = static c => c.Response.ContentType;
        return true;
    }

    public override IStatement ConvertFieldStatement(FieldStatement field)
    {
        return new HttpTemplateFuncFieldStatement(field.Names, c => c.Response.ContentType);
    }
}