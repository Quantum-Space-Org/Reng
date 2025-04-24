namespace Reng.BPMN.Domain.Domain;

public class InputDataProvider
{
    public string Id = Guid.NewGuid().ToString();
    public static InputDataProvider Default() => new InputDataProvider(DataProviderType.Nothing);
    private readonly DataProviderType _source;
    private InputDataProvider(){}
    public InputDataProvider(DataProviderType source)
    {
        _source = source;
    }

    public DataProviderType GetType()
    {
        return _source;
    }
}