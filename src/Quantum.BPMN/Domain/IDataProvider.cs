namespace Reng.BPMN.Domain.Domain;

public interface IDataProvider
{
    object Get(string name);
}

public class NullDataProvider : IDataProvider
{
    public object Get(string name)
    {
        return null;
    }

    public static IDataProvider New()
    {
        return new NullDataProvider();
    }
}