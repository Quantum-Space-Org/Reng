namespace Reng.BPMN.Domain.Domain;

public record ManualDataProvider
{
    private readonly Dictionary<string, object> _properties;

    public ManualDataProvider()
        => _properties = new();

    public void SetProperty(string name, FieldType type, object value)
        => _properties[name] = value;

    public object GetProperty(string name)
        => _properties[name];
}

public enum FieldType
{
    String,
    Int,
    Float,
    List,
    Dictionary
}