namespace Reng.BPMN.Domain.Domain;

public class BpmnExecutionContext
{
    private readonly Dictionary<string, object> _dictionary;

    internal Dictionary<string, object> GetDictionary() { return _dictionary; }
    public BpmnExecutionContext()
    {
        _dictionary = new Dictionary<string, object>();
    }
    public void Populate(Dictionary<string, object> dictionary)
    {
        foreach (var o in dictionary)
        {
            _dictionary[o.Key] = o.Value;
        }
    }

    public object Get(string name)
    {
        return _dictionary[name];
    }

    public void Add(string name, string value)
    {
        _dictionary[name] = value;
    }
}