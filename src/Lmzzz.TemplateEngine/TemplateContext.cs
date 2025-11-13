using System.Text;

namespace Lmzzz.Template.Inner;

public class TemplateContext
{
    public object Data { get; }

    public IDictionary<string, object> Cache { get; }

    public Dictionary<string, Func<TemplateContext, IStatement[], object?>> Functions { get; set; }

    public StringBuilder StringBuilder { get; set; }

    public TemplateContext(object data)
    {
        Data = data;
        Cache = new Dictionary<string, object>();
    }

    public TemplateContext(object data, IDictionary<string, object> cache, Dictionary<string, Func<TemplateContext, IStatement[], object?>> funcs, StringBuilder stringBuilder)
    {
        Data = data;
        Functions = funcs;
        Cache = new CacheMap<string, object>(cache);
        StringBuilder = stringBuilder;
    }

    public TemplateContext Scope()
    {
        return new TemplateContext(Data, Cache, Functions, StringBuilder);
    }
}