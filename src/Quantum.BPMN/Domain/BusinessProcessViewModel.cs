namespace Reng.BPMN.Domain.Domain;

public class BusinessProcessViewModel
{
    public string Name { get; set; }
    public string Id { get; set; }
    public BusinessProcessStatus Status { get; set; }
    public BusinessProcessViewModel(string id, string name , BusinessProcessStatus status)
    {
        Id = id;
        Name = name;
        Status = status;
    }
}