namespace Reng.BPMN.Domain.Domain;

public record IAmAnEndEvent(string Id, string Name) : IAmAnEvent(Id, Name)
{
    public override IAmABusinessProcessElement GetNext()
    {
        return null;
    }

    public override void SetOutgoing(IAmABusinessProcessElement outgoing)
    {
        if (OutgoingList.Count == 1)
            throw new AddMoreThanOneOutgoingToEndEventException("رویداد پایانی نمی تواند خروجی داشته باشد.");

        base.SetOutgoing(outgoing);
    }
}

public class AddMoreThanOneOutgoingToEndEventException : ObjectNotFoundException
{
    public AddMoreThanOneOutgoingToEndEventException(string message) : base(message)
    {
    }

    public AddMoreThanOneOutgoingToEndEventException(string message, Exception exception) : base(message, exception)
    {
    }
}