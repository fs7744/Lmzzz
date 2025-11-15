namespace Lmzzz.Template.Inner;

public class LessThenStatement : IOperaterStatement
{
    public IStatement Left { get; }

    public string Operater => "<";

    public IStatement Right { get; }

    public LessThenStatement(IStatement left, IStatement right)
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
        if (l is null || r is null)
            return false;
        if (l is decimal dl)
        {
            return dl < Convert.ToDecimal(r);
        }
        else if (l is int il)
        {
            return il < Convert.ToInt32(r);
        }
        else if (l is long ll)
        {
            return ll < Convert.ToInt64(r);
        }
        else if (l is double ddl)
        {
            return ddl < Convert.ToDouble(r);
        }
        else if (l is float fl)
        {
            return fl < Convert.ToSingle(r);
        }
        else if (l is DateTime tl)
        {
            return tl < Convert.ToDateTime(r);
        }
        else if (l is short sl)
        {
            return sl < Convert.ToInt16(r);
        }
        else if (l is byte bl)
        {
            return bl < Convert.ToByte(r);
        }
        else if (l is sbyte sbl)
        {
            return sbl < Convert.ToSByte(r);
        }
        else if (l is char cl)
        {
            return cl < Convert.ToChar(r);
        }
        else if (l is ushort ul)
        {
            return ul < Convert.ToUInt16(r);
        }
        else if (l is uint uil)
        {
            return uil < Convert.ToUInt32(r);
        }
        else if (l is ulong ull)
        {
            return ull < Convert.ToUInt64(r);
        }
        return false;
    }
}