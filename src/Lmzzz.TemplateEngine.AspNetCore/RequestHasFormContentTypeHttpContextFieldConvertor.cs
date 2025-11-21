using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public class RequestHasFormContentTypeHttpContextFieldConvertor : HttpContextFieldConvertor
{
    private readonly IHttpConditionStatement isTrue;
    private readonly IHttpConditionStatement isFalse;

    public RequestHasFormContentTypeHttpContextFieldConvertor()
    {
        isTrue = CreateAction(c => c.Request.HasFormContentType);
        isFalse = CreateAction(c => !c.Request.HasFormContentType);
    }

    public override IHttpConditionStatement ConvertEqual(IStatement statement)
    {
        if (DefaultTemplateEngineFactory.TryGetBool(statement, out var b))
        {
            return b ? isTrue : isFalse;
        }
        else if (statement is NullValueStatement || statement is StringValueStatement || statement is DecimalValueStatement)
            return AlwaysFalse;
        else if (DefaultTemplateEngineFactory.TryGetBoolFunc(statement, out var f))
            return CreateAction(c => f(c) == c.Request.HasFormContentType);
        else
            return null;
    }

    public override string Key()
    {
        return "field_Request.HasFormContentType";
    }

    public override bool TryConvertBoolFunc(IStatement statement, out Func<HttpContext, bool> func)
    {
        func = static c => c.Request.HasFormContentType;
        return true;
    }

    public override bool TryConvertStringFunc(IStatement statement, out Func<HttpContext, string> func)
    {
        func = static c => c.Request.HasFormContentType.ToString();
        return true;
    }

    public override IStatement ConvertFieldStatement(FieldStatement field)
    {
        return new HttpTemplateFuncFieldStatement(field.Names, c => c.Request.HasFormContentType);
    }
}