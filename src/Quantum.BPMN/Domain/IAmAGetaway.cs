namespace Reng.BPMN.Domain.Domain;

public abstract record IAmAGetaway(string Id, string Name) : IAmABusinessProcessElement(Id, Name)
{
    public abstract ICollection<IAmABusinessProcessElement> GetOutgoingList();

    public abstract bool CanNotGoFurther();
}

public record IAmAnExclusiveGetaway(string Id, string Name) : IAmAGetaway(Id, Name)
{
    public override IAmABusinessProcessElement GetNext()
        => OutgoingList.First();

    public override ICollection<IAmABusinessProcessElement> GetOutgoingList()
    {
        throw new NotImplementedException();
    }

    public override bool CanNotGoFurther()
    {
        throw new NotImplementedException();
    }
}
public record IAmAParallelGetaway(string Id, string Name) : IAmAGetaway(Id, Name)
{
    public override IAmABusinessProcessElement GetNext()
        => OutgoingList.First().GetNext();

    public override ICollection<IAmABusinessProcessElement> GetOutgoingList()
        => OutgoingList;

    public override bool CanNotGoFurther() 
        => IncomingList.Any(a => a.Status != BusinessProcessElementStatus.CompletedSuccessfully);
}

public record IAmAnInclusiveGetaway(string Id, string Name) : IAmAGetaway(Id, Name)
{
    public override IAmABusinessProcessElement GetNext()
        => OutgoingList.First();

    public override ICollection<IAmABusinessProcessElement> GetOutgoingList()
    {
        throw new NotImplementedException();
    }

    public override bool CanNotGoFurther()
    {
        throw new NotImplementedException();
    }
}