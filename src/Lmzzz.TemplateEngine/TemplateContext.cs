using System.Text;

namespace Lmzzz.Template.Inner;

public class TemplateContext
{
    public object Data { get; }

    public IDictionary<string, object> Cache { get; private set; }
    internal int scopeCount;

    public StringBuilder StringBuilder { get; set; }

    public FieldStatementMode FieldMode { get; set; }

    public TemplateContext(object data)
    {
        Data = data;
    }

    public void Scope()
    {
        scopeCount++;
        if (Cache == null)
            Cache = new Dictionary<string, object>();
    }

    internal void LeaveScope()
    {
        scopeCount--;
        if (Cache != null)
            Cache.Clear();
    }
}