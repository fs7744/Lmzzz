using Lmzzz.Template.Inner;
using Microsoft.AspNetCore.Http;

namespace Lmzzz.AspNetCoreTemplate;

public abstract class HttpContextFieldConvertor
{
    public static readonly IHttpConditionStatement AlwaysFalse = new BoolConditionStatement(false);
    public static readonly IHttpConditionStatement AlwaysTrue = new BoolConditionStatement(true);
    public static readonly Func<HttpContext, bool> AlwaysFalseFunc = c => false;
    public static readonly Func<HttpContext, bool> AlwaysTrueFunc = c => true;

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