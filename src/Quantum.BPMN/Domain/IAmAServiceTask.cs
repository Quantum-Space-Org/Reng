namespace Reng.BPMN.Domain.Domain;

public record IAmAServiceTask(string Id, string Name) : IAmABusinessProcessElement(Id, Name)
{
    public TaskExecutorDescription TaskExecutorDescription { get; private set; } = TaskExecutorDescription.Default();
    public CompensateExecution CompensateDescription { get; private set; } = CompensateExecution.Default();
    public InputDataProvider InputDataProvider { get; private set; } = InputDataProvider.Default();

    public override IAmABusinessProcessElement GetNext()
        => OutgoingList.First();

    public override void SetOutgoing(IAmABusinessProcessElement outgoing)
    {
        if (OutgoingList.Count == 1)
            throw new AddMoreThanOneOutgoingToUserTaskException("سرویس تسک نمی تواند بیش از یک خروجی داشته باشد");

        base.SetOutgoing(outgoing);
    }

    public void SetExecutorDescription(TaskExecutorDescription description)
        => TaskExecutorDescription = description;

    public void SetDataProvider(InputDataProvider inputDataProvider)
        => InputDataProvider = inputDataProvider;

    public void SetCompensate(CompensateExecution compensateExecution)
    {
        if (HasCompensate is false)
            throw new RengDomainException("این تسک دارای باندری ایونت از نوع کامپنسنت نمی باشد!");

        CompensateDescription = compensateExecution;
    }

    internal void AddTaskDataProvider(DataProviderType type)
    {
        InputDataProvider = new InputDataProvider(type);
    }
}

public enum TaskExecutorType
{
    NotSetYet,
    RestApi
}

public class AddMoreThanOneOutgoingToUserTaskException : ObjectNotFoundException
{
    public AddMoreThanOneOutgoingToUserTaskException(string message) : base(message)
    {
    }

    public AddMoreThanOneOutgoingToUserTaskException(string message, Exception exception) : base(message, exception)
    {
    }
}