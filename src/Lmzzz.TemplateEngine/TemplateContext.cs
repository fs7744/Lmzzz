using System.Text;

namespace Lmzzz.Template.Inner;

public class TemplateContext
{
    public object Data { get; }

    public IDictionary<string, object> Cache { get; private set; }
    internal int scopeCount;

    public StringBuilder StringBuilder { get; set; }

    public TemplateContext(object data)
    {
        Data = data;
        //Cache = new Dictionary<string, object>();
    }

    //public TemplateContext(object data, IDictionary<string, object> cache, StringBuilder stringBuilder)
    //{
    //    Data = data;
    //    Cache = new CacheMap<string, object>(cache);
    //    StringBuilder = stringBuilder;
    //}

    public void Scope()
    {
        scopeCount++;
        if (Cache == null)
            Cache = new Dictionary<string, object>();
        //return new TemplateContext(Data, Cache, StringBuilder);
    }

    internal void LeaveScope()
    {
        scopeCount--;
        if (Cache != null)
            Cache.Clear();
    }
}