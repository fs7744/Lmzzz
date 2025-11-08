using System.Text;

namespace Lmzzz.Template.Inner;

public class TemplateContext
{
    public object Data { get; }

    public Dictionary<string, object> Cache { get; }

    public Dictionary<string, Func<TemplateContext, IStatement[], object?>> Functions { get; set; }

    public StringBuilder StringBuilder { get; set; }

    public TemplateContext(object data)
    {
        Data = data;
        Cache = new Dictionary<string, object>();
    }
}