using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public class ResponseContentLengthHttpContextFieldConvertor : HttpContextFieldConvertor
{
    private readonly IHttpConditionStatement isnull;

    public ResponseContentLengthHttpContextFieldConvertor()
    {
        isnull = CreateAction(c => !c.Response.ContentLength.HasValue);
    }

    public override IHttpConditionStatement ConvertEqual(IStatement statement)
    {
        if (TryGetDecimal(statement, out var d))
        {
            return CreateAction(c => d == c.Response.ContentLength);
        }
        else if (statement is NullValueStatement)
            return isnull;
        else if (statement is StringValueStatement || statement is BoolValueStatement)
            return AlwaysFalse;
        else
            return null;
    }

    public override string Key()
    {
        return "field_Response.ContentLength";
    }

    public override bool TryConvertBoolFunc(IStatement statement, out Func<HttpContext, bool> func)
    {
        func = AlwaysFalseFunc;
        return true;
    }

    public override bool TryConvertStringFunc(IStatement statement, out Func<HttpContext, string> func)
    {
        func = static c => c.Response.ContentLength?.ToString();
        return true;
    }
}