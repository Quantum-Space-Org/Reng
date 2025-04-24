using Reng.BPMN.ACL;

namespace Reng.BPMN.Domain.Domain;

public class BusinessProcess
{
    public string Id { get; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; }

    public readonly IAmAnStartEvent StartEvent;
    public readonly IAmAnEndEvent EndEvent;
    public readonly List<IAmAServiceTask> ServiceTasks;
    public readonly List<IAmAnInclusiveGetaway> InclusiveGetaways;
    public readonly List<IAmAnExclusiveGetaway> ExclusiveGetaway;
    public readonly List<IAmAParallelGetaway> ParallelGetaways;

    private BusinessProcess() { }

    public BusinessProcess(string id
                           , string name
                           , IAmAnStartEvent startAnAnEvent
                           , IAmAnEndEvent endEvent
                           , List<IAmAServiceTask> serviceTasks
                           , List<IAmAnInclusiveGetaway> inclusiveGetaways
                           , List<IAmAnExclusiveGetaway> exclusiveGetaway
                           , List<IAmAParallelGetaway> parallelGetaways
                           , List<SequenceFlow> sequenceFlows
                           , List<IAmABoundaryEvent> boundaryEvents)
    {
        Id = id;
        Name = name;

        StartEvent = startAnAnEvent ?? throw new RengDomainException("فرآیند بیزنسی باید شامل ایونت شروع باشد");
        EndEvent = endEvent ?? throw new RengDomainException("فرآیند بیزنسی باید شامل ایونت پایان باشد");
        ServiceTasks = serviceTasks;
        InclusiveGetaways = inclusiveGetaways;
        ExclusiveGetaway = exclusiveGetaway;
        ParallelGetaways = parallelGetaways;

        ConnectBusinessElements(sequenceFlows);
        AttachBoundaryEventsToElements(boundaryEvents);


        CreatedAt = DateTime.UtcNow;
    }

    public void AddTaskCompensation(string taskId, CompensateExecution compensateExecution)
    {
        var task = GetUserTask(taskId);

        task.SetCompensate(compensateExecution);
    }

    public void AddTaskDataProvider(string taskId, DataProviderType type)
    {
        var task = GetUserTask(taskId);

        task.AddTaskDataProvider(type);
    }

    private void AttachBoundaryEventsToElements(List<IAmABoundaryEvent> boundaryEvents)
    {
        if (boundaryEvents is not null)
            foreach (var item in boundaryEvents)
            {
                var element = FindBusinessElementById(item.AttachTo);

                if (item.HasCompensate)
                    element.SetHasCompensate(true);
            }
    }


    private void ConnectBusinessElements(List<SequenceFlow> sequenceFlows)
    {
        foreach (var sequenceFlow in sequenceFlows)
        {
            var sourceRef = sequenceFlow.SourceRef;
            var targetRef = sequenceFlow.TargetRef;

            var source = FindBusinessElementById(sourceRef);
            var target = FindBusinessElementById(targetRef);

            source.SetOutgoing(target);
            target.SetIncoming(source);
        }
    }

    private IAmABusinessProcessElement FindBusinessElementById(string referenceId)
    {
        if (StartEvent is not null && StartEvent.Id == referenceId)
            return StartEvent;

        if (EndEvent?.Id == referenceId)
            return EndEvent;

        if (ServiceTasks is not null && ServiceTasks.Exists(ut => ut.Id == referenceId))
            return ServiceTasks.FirstOrDefault(ut => ut.Id == referenceId);

        if (InclusiveGetaways is not null && InclusiveGetaways.Exists(ut => ut.Id == referenceId))
            return InclusiveGetaways.FirstOrDefault(ut => ut.Id == referenceId);

        if (ExclusiveGetaway is not null && ExclusiveGetaway.Exists(ut => ut.Id == referenceId))
            return ExclusiveGetaway.FirstOrDefault(ut => ut.Id == referenceId);

        if (ParallelGetaways is not null && ParallelGetaways.Exists(ut => ut.Id == referenceId))
            return ParallelGetaways.FirstOrDefault(ut => ut.Id == referenceId);

        throw new ObjectNotFoundException("آبجکتی با ای دی " + referenceId + " یافت نشد");
    }

    public IAmAnEvent GetStartEvent()
        => StartEvent;

    public IAmAnEvent GetEndEvent() 
        => EndEvent;

    public List<IAmAServiceTask> GetUserTasks()
        => ServiceTasks;

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

    public BusinessProcessInstance CreateInstance()
    {
        return new BusinessProcessInstance(Guid.NewGuid().ToString(), this.Id)
        {
            StartEvent = this.StartEvent,
            EndEvent = this.EndEvent,
            ExclusiveGetaway = ExclusiveGetaway,
            InclusiveGetaways = InclusiveGetaways,
            ParallelGetaways = ParallelGetaways,
            ServiceTasks = ServiceTasks,
        };
    }
}

public class BusinessProcessLog
{
    public string BusinessProcessInstanceId { get; set; }
    public string ElementId { get; set; }

    public string ElementName { get; set; }
    public BusinessProcessElementStatus Status { get; set; }
    public BpmnExecutionContext Context { get; set; }
    public DateTime DateTime { get; set; }
    public string Message { get; set; }
    public string Id { get; set; }
}

public enum BusinessProcessStatus
{
    NotStarted,
    InProgress,
    Pending,
    Failed,
    CompletedSuccessfully
}