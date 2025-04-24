namespace Reng.BPMN.Domain.Domain;

public record IAmAnStartEvent(string Id, string Name) : IAmAnEvent(Id, Name)
{
    public override IAmABusinessProcessElement GetNext()
        => OutgoingList.First();
    
    public override void SetOutgoing(IAmABusinessProcessElement outgoing)
    {
        GuardAgainstAddingMoreThanOneOutgoing();

        base.SetOutgoing(outgoing);
    }

    private void GuardAgainstAddingMoreThanOneOutgoing()
    {
        if (OutgoingList.Count == 1)
            throw new AddMoreThanOneOutgoingToStartEventException("رویداد شروع نمی تواند بیش از یک خروجی داشته باشد");
    }

    public override void SetIncoming(IAmABusinessProcessElement outgoing)
        => throw new AddIncomingToStartEventException("رویداد شروع نمی تواند ورودی داشته باشد");
}


public class AddIncomingToStartEventException : ObjectNotFoundException
{
    public AddIncomingToStartEventException(string message) : base(message)
    {
    }

    public AddIncomingToStartEventException(string message, Exception exception) : base(message, exception)
    {
    }
}
public class AddMoreThanOneOutgoingToStartEventException : ObjectNotFoundException
{
    public AddMoreThanOneOutgoingToStartEventException(string message) : base(message)
    {
    }

    public AddMoreThanOneOutgoingToStartEventException(string message, Exception exception) : base(message, exception)
    {
    }
}