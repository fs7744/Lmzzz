using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public abstract class GenericHttpContextFieldConvertor : HttpContextFieldConvertor
{
    public override IHttpConditionStatement ConvertEqual(IStatement statement)
    {
        return null;
    }

    public override bool TryConvertBoolFunc(IStatement statement, out Func<HttpContext, bool> func)
    {
        func = null;
        return false;
    }

    public override bool TryConvertStringFunc(IStatement statement, out Func<HttpContext, string> func)
    {
        func = null;
        return false;
    }

    public virtual IHttpConditionStatement GenericConvertEqual(FieldStatement field, IStatement statement, Dictionary<string, HttpTemplateFuncFieldStatement> fields)
    {
        return null;
    }
}

public class ItemsGenericHttpContextFieldConvertor : GenericHttpContextFieldConvertor
{
    public override string Key()
    {
        return "field_Items.";
    }
}