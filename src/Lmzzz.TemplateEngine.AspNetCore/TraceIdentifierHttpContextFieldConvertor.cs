using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public class TraceIdentifierHttpContextFieldConvertor : HttpContextFieldConvertor
{
    private readonly IHttpConditionStatement isnull;

    public TraceIdentifierHttpContextFieldConvertor()
    {
        isnull = CreateAction(c => c.TraceIdentifier is null);
    }

    public override IHttpConditionStatement ConvertEqual(IStatement statement)
    {
        if (DefaultTemplateEngineFactory.TryGetString(statement, out var str))
        {
            return CreateAction(c => str == c.TraceIdentifier);
        }
        else if (statement is NullValueStatement)
            return isnull;
        else if (DefaultTemplateEngineFactory.TryGetStringFunc(statement, out var f))
            return CreateAction(c => f(c) == c.TraceIdentifier);
        else
            return null;
    }

    public override string Key()
    {
        return "field_TraceIdentifier";
    }

    public override bool TryConvertBoolFunc(IStatement statement, out Func<HttpContext, bool> func)
    {
        func = AlwaysFalseFunc;
        return true;
    }

    public override bool TryConvertStringFunc(IStatement statement, out Func<HttpContext, string> func)
    {
        func = static c => c.TraceIdentifier;
        return true;
    }

    public override IStatement ConvertFieldStatement(FieldStatement field)
    {
        return new HttpTemplateFuncFieldStatement(field.Names, c => c.TraceIdentifier);
    }
}