using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public abstract class HttpContextFieldConvertor
{
    public static readonly IHttpConditionStatement AlwaysFalse = new BoolConditionStatement(false);
    public static readonly IHttpConditionStatement AlwaysTrue = new BoolConditionStatement(true);

    public bool TryGetString(IStatement statement, out string str)
    {
        if (statement is StringValueStatement s)
        {
            str = s.Value;
            return true;
        }
        else if (statement is BoolValueStatement b)
        {
            str = b.Value.ToString();
            return true;
        }
        else if (statement is DecimalValueStatement d)
        {
            str = d.Value.ToString();
            return true;
        }
        else if (statement == AlwaysFalse)
        {
            str = false.ToString();
            return true;
        }
        else if (statement == AlwaysTrue)
        {
            str = true.ToString();
            return true;
        }

        str = null;
        return false;
    }

    public IHttpConditionStatement CreateAction(Func<HttpContext, bool> action)
    {
        return new ActionConditionStatement(action);
    }

    public abstract IHttpConditionStatement ConvertEqual(IStatement statement);

    public abstract bool TryConvertStringFunc(IStatement statement, out Func<HttpContext, string> func);
}