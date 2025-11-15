namespace Lmzzz.Template.Inner;

public class EqualStatement : IOperaterStatement
{
    public static readonly Dictionary<Type, Func<object, object, bool>> EqualityComparers = new Dictionary<Type, Func<object, object, bool>>();

    public IStatement Left { get; }

    public string Operater => "==";

    public IStatement Right { get; }

    public EqualStatement(IStatement left, IStatement right)
    {
        Left = left;
        Right = right;
    }

    public object? Evaluate(TemplateContext context)
    {
        return EvaluateCondition(context);
    }

    public bool EvaluateCondition(TemplateContext context)
    {
        var l = Left.Evaluate(context);
        var r = Right.Evaluate(context);
        if (l is null)
            return r is null;
        if (r is null)
            return false;
        if (EqualityComparers.TryGetValue(l.GetType(), out var eq))
        {
            return eq(l, r);
        }
        else if (EqualityComparers.TryGetValue(r.GetType(), out eq))
        {
            return eq(r, l);
        }
        else if (l is decimal dl)
        {
            return dl == Convert.ToDecimal(r);
        }
        else if (l is int il)
        {
            return il == Convert.ToInt32(r);
        }
        else if (l is bool bbl)
        {
            return bbl == Convert.ToBoolean(r);
        }
        else if (l is string ils)
        {
            return ils == Convert.ToString(r);
        }
        else if (l is long ll)
        {
            return ll == Convert.ToInt64(r);
        }
        else if (l is double ddl)
        {
            return ddl == Convert.ToDouble(r);
        }
        else if (l is float fl)
        {
            return fl == Convert.ToSingle(r);
        }
        else if (l is DateTime tl)
        {
            return tl == Convert.ToDateTime(r);
        }
        else if (l is short sl)
        {
            return sl == Convert.ToInt16(r);
        }
        else if (l is byte bl)
        {
            return bl == Convert.ToByte(r);
        }
        else if (l is sbyte sbl)
        {
            return sbl == Convert.ToSByte(r);
        }
        else if (l is char cl)
        {
            return cl == Convert.ToChar(r);
        }
        else if (l is ushort ul)
        {
            return ul == Convert.ToUInt16(r);
        }
        else if (l is uint uil)
        {
            return uil == Convert.ToUInt32(r);
        }
        else if (l is ulong ull)
        {
            return ull == Convert.ToUInt64(r);
        }
        return object.Equals(l, r);
    }
}