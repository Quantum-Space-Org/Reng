namespace Reng.BPMN.Domain;

public class RengDomainException : Exception
{
    public RengDomainException(string message) : base(message)
    {
    }
    public RengDomainException(string message, Exception exception) : base(message, exception)
    {
    }
}

public class ObjectNotFoundException : RengDomainException
{
    public ObjectNotFoundException(string message) : base(message)
    {
    }

    public ObjectNotFoundException(string message, Exception exception) : base(message, exception)
    {
    }
}