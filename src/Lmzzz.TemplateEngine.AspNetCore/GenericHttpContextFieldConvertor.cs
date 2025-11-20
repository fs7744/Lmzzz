using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public abstract class GenericHttpContextFieldConvertor : HttpContextFieldConvertor
{
    public override bool IsGeneric() => true;

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
}

public class ItemsGenericHttpContextFieldConvertor : GenericHttpContextFieldConvertor
{
    public override string Key()
    {
        return "field_Items.";
    }
}