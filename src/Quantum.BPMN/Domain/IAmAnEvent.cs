namespace Reng.BPMN.Domain.Domain;

public abstract record IAmAnEvent(string Id, string Name) : IAmABusinessProcessElement(Id, Name)
{
}