using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;
using System;

namespace Lmzzz.AspNetCoreTemplate;

public class RequestHostHttpContextFieldConvertor : HttpContextFieldConvertor
{
    private readonly IHttpConditionStatement isnull;

    public RequestHostHttpContextFieldConvertor()
    {
        isnull = CreateAction(c => c.Request.Host.Value is null);
    }

    public override IHttpConditionStatement ConvertEqual(IStatement statement)
    {
        if (TryGetString(statement, out var str))
        {
            return CreateAction(c => str == c.Request.Host.Value);
        }
        else if (statement is NullValueStatement)
            return isnull;
        else if (DefaultTemplateEngineFactory.TryGetStringFunc(statement, out var f))
            return CreateAction(c => f(c) == c.Request.Host.Value);
        else
            return null;
    }

    public override string Key()
    {
        return "field_Request.Host";
    }

    public override bool TryConvertBoolFunc(IStatement statement, out Func<HttpContext, bool> func)
    {
        func = AlwaysFalseFunc;
        return true;
    }

    public override bool TryConvertStringFunc(IStatement statement, out Func<HttpContext, string> func)
    {
        func = static c => c.Request.Host.Value;
        return true;
    }
}