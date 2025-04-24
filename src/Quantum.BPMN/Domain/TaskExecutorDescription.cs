namespace Reng.BPMN.Domain.Domain;

public class TaskExecutorDescription
{
    public TaskExecutorType Type { get; set; }
    public string Url { get; set; }

    public static TaskExecutorDescription Default() => new() { Type = TaskExecutorType.NotSetYet };
}

public class CompensateExecution : TaskExecutorDescription
{
    public static CompensateExecution Default() => new() { Type = TaskExecutorType.NotSetYet };

}