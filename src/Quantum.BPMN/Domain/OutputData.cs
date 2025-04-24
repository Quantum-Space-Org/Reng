namespace Reng.BPMN.Domain.Domain;

public class OutputData
{
    private readonly Dictionary<string, object> _properties;

    public OutputData()
        => _properties = new Dictionary<string, object>();

    public void Set(string name, string value, FieldType fieldType)
        => _properties[name] = value;

    public object Get(string name)
        => _properties[name];
}