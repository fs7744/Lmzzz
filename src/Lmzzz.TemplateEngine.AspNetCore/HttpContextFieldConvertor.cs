using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public abstract class HttpContextFieldConvertor
{
    public static readonly IHttpConditionStatement AlwaysFalse = new BoolConditionStatement(false);
    public static readonly IHttpConditionStatement AlwaysTrue = new BoolConditionStatement(true);
    public static readonly Func<HttpContext, bool> AlwaysFalseFunc = c => false;
    public static readonly Func<HttpContext, bool> AlwaysTrueFunc = c => true;

    public virtual bool IsGeneric() => false;

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

    public bool TryGetBool(IStatement statement, out bool b)
    {
        if (statement is BoolValueStatement bb)
        {
            b = bb.Value;
            return true;
        }
        else if (statement == AlwaysFalse)
        {
            b = false;
            return true;
        }
        else if (statement == AlwaysTrue)
        {
            b = true;
            return true;
        }

        b = false;
        return false;
    }

    public bool TryGetDecimal(IStatement statement, out decimal d)
    {
        if (statement is DecimalValueStatement dd)
        {
            d = dd.Value;
            return true;
        }
        else if (statement is StringValueStatement s)
        {
            return Decimal.TryParse(s.Value, out d);
        }

        d = 0;
        return false;
    }

    public IHttpConditionStatement CreateAction(Func<HttpContext, bool> action)
    {
        return new ActionConditionStatement(action);
    }

    public abstract string Key();

    public abstract IHttpConditionStatement ConvertEqual(IStatement statement);

    public abstract bool TryConvertStringFunc(IStatement statement, out Func<HttpContext, string> func);

    public abstract bool TryConvertBoolFunc(IStatement statement, out Func<HttpContext, bool> func);

    public virtual IStatement ConvertFieldStatement(FieldStatement field) => null;
}