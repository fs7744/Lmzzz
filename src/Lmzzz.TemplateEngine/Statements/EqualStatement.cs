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
        return Eqs(l, r);
    }

    public static bool Eqs(object? l, object? r)
    {
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
        else if (l is string s)
        {
            return string.Equals(l, r?.ToString());
        }
        else if (l is decimal dl)
        {
            if (r is decimal dr)
                return dl == dr;
            else if (r is int di)
                return dl == di;
            else if (r is long bbl)
                return dl == Convert.ToDecimal(bbl);
            else if (r is double bbd)
                return dl == Convert.ToDecimal(bbd);
            else if (r is float bbf)
                return dl == Convert.ToDecimal(bbf);
            else if (r is short bbs)
                return dl == Convert.ToDecimal(bbs);
            else if (r is byte bbb)
                return dl == Convert.ToDecimal(bbb);
            else if (r is sbyte bbsb)
                return dl == Convert.ToDecimal(bbsb);
            else if (r is char bbc)
                return dl == Convert.ToDecimal(bbc);
            else if (r is ushort bbus)
                return dl == Convert.ToDecimal(bbus);
            else if (r is uint bbui)
                return dl == Convert.ToDecimal(bbui);
            else if (r is ulong bbul)
                return dl == Convert.ToDecimal(bbul);
        }
        else if (l is int di)
        {
            if (r is decimal dr)
                return di == dr;
            else if (r is int dii)
                return di == dii;
            else if (r is long bbl)
                return di == bbl;
            else if (r is double bbd)
                return di == bbd;
            else if (r is float bbf)
                return di == bbf;
            else if (r is short bbs)
                return di == bbs;
            else if (r is byte bbb)
                return di == bbb;
            else if (r is sbyte bbsb)
                return di == bbsb;
            else if (r is char bbc)
                return di == bbc;
            else if (r is ushort bbus)
                return di == bbus;
            else if (r is uint bbui)
                return di == bbui;
        }
        else if (l is long dll)
        {
            if (r is decimal dr)
                return dll == dr;
            else if (r is int dlli)
                return dll == dlli;
            else if (r is long bbl)
                return dll == bbl;
            else if (r is double bbd)
                return dll == bbd;
            else if (r is float bbf)
                return dll == bbf;
            else if (r is short bbs)
                return dll == bbs;
            else if (r is byte bbb)
                return dll == bbb;
            else if (r is sbyte bbsb)
                return dll == bbsb;
            else if (r is char bbc)
                return dll == bbc;
            else if (r is ushort bbus)
                return dll == bbus;
            else if (r is uint bbui)
                return dll == bbui;
        }
        else if (l is double dld)
        {
            if (r is decimal dr)
                return Convert.ToDecimal(dld) == dr;
            else if (r is int dldi)
                return dld == dldi;
            else if (r is long bbl)
                return dld == bbl;
            else if (r is double bbd)
                return dld == bbd;
            else if (r is float bbf)
                return dld == bbf;
            else if (r is short bbs)
                return dld == bbs;
            else if (r is byte bbb)
                return dld == bbb;
            else if (r is sbyte bbsb)
                return dld == bbsb;
            else if (r is char bbc)
                return dld == bbc;
            else if (r is ushort bbus)
                return dld == bbus;
            else if (r is uint bbui)
                return dld == bbui;
            else if (r is ulong bbul)
                return dld == bbul;
        }
        else if (l is float dlf)
        {
            if (r is decimal dr)
                return Convert.ToDecimal(dlf) == dr;
            else if (r is int dlfi)
                return dlf == dlfi;
            else if (r is long bbl)
                return dlf == bbl;
            else if (r is double bbd)
                return dlf == bbd;
            else if (r is float bbf)
                return dlf == bbf;
            else if (r is short bbs)
                return dlf == bbs;
            else if (r is byte bbb)
                return dlf == bbb;
            else if (r is sbyte bbsb)
                return dlf == bbsb;
            else if (r is char bbc)
                return dlf == bbc;
            else if (r is ushort bbus)
                return dlf == bbus;
            else if (r is uint bbui)
                return dlf == bbui;
            else if (r is ulong bbul)
                return dlf == bbul;
        }
        else if (l is short dls)
        {
            if (r is decimal dr)
                return dls == dr;
            else if (r is int dlsi)
                return dls == dlsi;
            else if (r is long bbl)
                return dls == bbl;
            else if (r is double bbd)
                return dls == bbd;
            else if (r is float bbf)
                return dls == bbf;
            else if (r is short bbs)
                return dls == bbs;
            else if (r is byte bbb)
                return dls == bbb;
            else if (r is sbyte bbsb)
                return dls == bbsb;
            else if (r is char bbc)
                return dls == bbc;
            else if (r is ushort bbus)
                return dls == bbus;
            else if (r is uint bbui)
                return dls == bbui;
        }
        else if (l is byte dlb)
        {
            if (r is decimal dr)
                return dlb == dr;
            else if (r is int dlbi)
                return dlb == dlbi;
            else if (r is long bbl)
                return dlb == bbl;
            else if (r is double bbd)
                return dlb == bbd;
            else if (r is float bbf)
                return dlb == bbf;
            else if (r is short bbs)
                return dlb == bbs;
            else if (r is byte bbb)
                return dlb == bbb;
            else if (r is sbyte bbsb)
                return dlb == bbsb;
            else if (r is char bbc)
                return dlb == bbc;
            else if (r is ushort bbus)
                return dlb == bbus;
            else if (r is uint bbui)
                return dlb == bbui;
            else if (r is ulong bbul)
                return dlb == bbul;
        }
        else if (l is sbyte dlsb)
        {
            if (r is decimal dr)
                return dlsb == dr;
            else if (r is int dlsbi)
                return dlsb == dlsbi;
            else if (r is long bbl)
                return dlsb == bbl;
            else if (r is double bbd)
                return dlsb == bbd;
            else if (r is float bbf)
                return dlsb == bbf;
            else if (r is short bbs)
                return dlsb == bbs;
            else if (r is byte bbb)
                return dlsb == bbb;
            else if (r is sbyte bbsb)
                return dlsb == bbsb;
            else if (r is char bbc)
                return dlsb == bbc;
            else if (r is ushort bbus)
                return dlsb == bbus;
            else if (r is uint bbui)
                return dlsb == bbui;
        }
        else if (l is char dlc)
        {
            if (r is decimal dr)
                return dlc == dr;
            else if (r is int dlci)
                return dlc == dlci;
            else if (r is long bbl)
                return dlc == bbl;
            else if (r is double bbd)
                return dlc == bbd;
            else if (r is float bbf)
                return dlc == bbf;
            else if (r is short bbs)
                return dlc == bbs;
            else if (r is byte bbb)
                return dlc == bbb;
            else if (r is sbyte bbsb)
                return dlc == bbsb;
            else if (r is char bbc)
                return dlc == bbc;
            else if (r is ushort bbus)
                return dlc == bbus;
            else if (r is uint bbui)
                return dlc == bbui;
            else if (r is ulong bbul)
                return dlc == bbul;
        }
        else if (l is ushort dlus)
        {
            if (r is decimal dr)
                return dlus == dr;
            else if (r is int dlusi)
                return dlus == dlusi;
            else if (r is long bbl)
                return dlus == bbl;
            else if (r is double bbd)
                return dlus == bbd;
            else if (r is float bbf)
                return dlus == bbf;
            else if (r is short bbs)
                return dlus == bbs;
            else if (r is byte bbb)
                return dlus == bbb;
            else if (r is sbyte bbsb)
                return dlus == bbsb;
            else if (r is char bbc)
                return dlus == bbc;
            else if (r is ushort bbus)
                return dlus == bbus;
            else if (r is uint bbui)
                return dlus == bbui;
            else if (r is ulong bbul)
                return dlus == bbul;
        }
        else if (l is uint dlui)
        {
            if (r is decimal dr)
                return dlui == dr;
            else if (r is int dluii)
                return dlui == dluii;
            else if (r is long bbl)
                return dlui == bbl;
            else if (r is double bbd)
                return dlui == bbd;
            else if (r is float bbf)
                return dlui == bbf;
            else if (r is short bbs)
                return dlui == bbs;
            else if (r is byte bbb)
                return dlui == bbb;
            else if (r is sbyte bbsb)
                return dlui == bbsb;
            else if (r is char bbc)
                return dlui == bbc;
            else if (r is ushort bbus)
                return dlui == bbus;
            else if (r is uint bbui)
                return dlui == bbui;
            else if (r is ulong bbul)
                return dlui == bbul;
        }
        else if (l is ulong dlul)
        {
            if (r is decimal dr)
                return dlul == dr;
            else if (r is double bbd)
                return dlul == bbd;
            else if (r is float bbf)
                return dlul == bbf;
            else if (r is byte bbb)
                return dlul == bbb;
            else if (r is char bbc)
                return dlul == bbc;
            else if (r is ushort bbus)
                return dlul == bbus;
            else if (r is uint bbui)
                return dlul == bbui;
            else if (r is ulong bbul)
                return dlul == bbul;
        }
        return object.Equals(l, r);
    }

    public void Visit(Action<IStatement> visitor)
    {
        if (Left is not null)
            visitor(Left);
        if (Right is not null)
            visitor(Right);
    }
}