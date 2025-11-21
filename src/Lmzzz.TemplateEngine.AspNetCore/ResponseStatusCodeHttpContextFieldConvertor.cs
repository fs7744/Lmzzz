using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public class ResponseStatusCodeHttpContextFieldConvertor : HttpContextFieldConvertor
{
    public override IHttpConditionStatement ConvertEqual(IStatement statement)
    {
        if (DefaultTemplateEngineFactory.TryGetDecimal(statement, out var d))
        {
            return CreateAction(c => d == c.Response.StatusCode);
        }
        else if (statement is NullValueStatement || statement is StringValueStatement || statement is BoolValueStatement)
            return AlwaysFalse;
        else
            return null;
    }

    public override string Key()
    {
        return "field_Response.StatusCode";
    }

    public override bool TryConvertBoolFunc(IStatement statement, out Func<HttpContext, bool> func)
    {
        func = AlwaysFalseFunc;
        return true;
    }

    public override bool TryConvertStringFunc(IStatement statement, out Func<HttpContext, string> func)
    {
        func = static c => c.Response.StatusCode.ToString();
        return true;
    }

    public override IStatement ConvertFieldStatement(FieldStatement field)
    {
        return new HttpTemplateFuncFieldStatement(field.Names, c => c.Response.StatusCode);
    }
}