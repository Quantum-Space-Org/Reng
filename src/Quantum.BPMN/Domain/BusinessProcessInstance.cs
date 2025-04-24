namespace Reng.BPMN.Domain.Domain;

public class BusinessProcessInstance
{
    public string Id { get; internal set; }
    public string BusinessProcessId { get; internal set; }
    public BusinessProcessStatus Status { get; internal set; }

    public IAmAnStartEvent StartEvent { get; internal set; }
    public IAmAnEndEvent EndEvent { get; internal set; }
    public List<IAmAServiceTask> ServiceTasks { get; internal set; }
    public List<IAmAnInclusiveGetaway> InclusiveGetaways { get; internal set; }
    public List<IAmAnExclusiveGetaway> ExclusiveGetaway { get; internal set; }
    public List<IAmAParallelGetaway> ParallelGetaways { get; internal set; }
    public BpmnExecutionContext Context;

    public DateTime InstantiatedAt { get; }

    public string Token;
    public List<BusinessProcessLog> Logs { get; private set; } = new();
    public Stack<IAmABusinessProcessElement> CompensateStack { get; set; } = new();

    private BusinessProcessInstance()
    {

    }
    public BusinessProcessInstance(string id, string businessProcessId)
    {
        Id = id;
        BusinessProcessId = businessProcessId;
        Token = $"TOKEN-{id}-{DateTime.UtcNow.ToLongDateString()}";
        Context = new BpmnExecutionContext();


        InstantiatedAt = DateTime.UtcNow;
    }

    public void FillContextWith(Dictionary<string, object>? data)
    {
        if (data != null)
            Context.Populate(data);
    }

    public BpmnExecutionContext GetContext() => Context;


    public IAmAnEvent GetStartEvent()
        => StartEvent;

    public IAmAnEvent GetEndEvent() => EndEvent;
    public List<IAmAServiceTask> GetUserTasks()
        => ServiceTasks;

    public IAmABusinessProcessElement GetNextElement()
    {
        if (StartEvent.Status == BusinessProcessElementStatus.NotStarted)
            return StartEvent;

        if (ParallelGetaways != null && ParallelGetaways.Any(ThatIsNotStartedOrHasTokenButFailed))
            return ParallelGetaways.First(ThatIsNotStartedOrHasTokenButFailed);

        if (ServiceTasks != null && ServiceTasks.Any(ThatIsNotStartedOrHasTokenButFailed))
            return ServiceTasks.First(ThatIsNotStartedOrHasTokenButFailed);

        return EndEvent;
    }

    private bool ThatIsNotStartedOrHasTokenButFailed(IAmABusinessProcessElement element)
    {
        return  element.Status == BusinessProcessElementStatus.NotStarted
                || (element.HasToken() && element.Status == BusinessProcessElementStatus.Failed);
    }

    internal void SetNextElement(IAmABusinessProcessElement element)
    {
        element?.SetToken(Token);
    }
    public void AddTaskDescription(string taskId, TaskExecutorDescription taskExecutorDescription)
    {
        var amAUserTask = GetUserTask(taskId);
        amAUserTask.SetExecutorDescription(taskExecutorDescription);
    }

    public IAmAServiceTask? GetTaskDescription(string taskId)
    {
        var amAUserTask = GetUserTask(taskId);

        return amAUserTask;
    }

    private IAmAServiceTask GetUserTask(string taskId)
    {
        var amAUserTask = this.ServiceTasks.FirstOrDefault(ut => ut.Id == taskId);

        if (amAUserTask == null)
            throw new ObjectNotFoundException("اکتیویتی با ای دی مورد نظر یافت نشد");
        return amAUserTask;
    }

    public void GoToInProgressState()
        => Status = BusinessProcessStatus.InProgress;
    public void GoToCompletedSuccessfullyState()
        => Status = BusinessProcessStatus.CompletedSuccessfully;

    public void GoToFailedState()
        => Status = BusinessProcessStatus.Failed;

    public void LogExecution(IAmABusinessProcessElement element, string message)
    {
        var businessProcessLogEntry = new BusinessProcessLog
        {
            Id = Guid.NewGuid().ToString(),
            BusinessProcessInstanceId = this.Id,
            ElementId = element.Id,
            ElementName = element.Name,
            Status = element.Status,
            Context = Context,
            DateTime = DateTime.UtcNow,
            Message = message
        };

        this.Logs.Add(businessProcessLogEntry);
    }

    public bool Finished()
        => !NotFinished();

    public bool NotFinished()
        => EndEvent.Status
            is not
            (BusinessProcessElementStatus.CompletedSuccessfully and BusinessProcessElementStatus.Failed);
}
