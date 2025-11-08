namespace Lmzzz.Template.Inner;

public class TemplateContext
{
    public object Data { get; }

    public Dictionary<string, object> Cache { get; }

    public TemplateContext(object data)
    {
        Data = data;
        Cache = new Dictionary<string, object>();
    }
}