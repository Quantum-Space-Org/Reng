using Reng.BPMN.Domain.Domain;

namespace Reng.BPMN.ApplicationService;

public interface IBpmnApplicationService
{
    Task<BusinessUnitResult> CreateBusinessProcess(string name, string bpmnContent);
    Task<List<BusinessProcessViewModel>> GetBusinessProcessList();

    Task AddTaskDescription(string name, string taskId, TaskExecutorDescription taskExecutorDescription);
    Task AddTaskCompensation(string name, string taskId, CompensateExecution compensateExecution);
    Task<TaskExecutionDescriptionViewModel> GetTaskDescription(string id, string taskId);

    Task<List<BusinessProcessLogViewModel>> GetBusinessProcessLogs(string id);
    Task<BusinessProcessDetailsViewModel> GetBusinessProcess(string name);
    Task<BusinessProcessInstanceDetailsViewModel> GetBusinessProcessInstance(string id);
    Task AddTaskDataProvider(string id, string taskId, DataProviderType type);
    Task<string> CreateInstanceFromBusinessProcess(string name);
    Task<List<BusinessProcessInstanceDetailsViewModel>> GetBusinessProcessInstances(string id);

    Task<ExecutionBusinessProcessStatus> ExecuteBusinessProcess(string id, Dictionary<string, object>? data);
    Task<ExecutionBusinessProcessStatus> ExecuteNewInstanceOfBusinessProcess(string name, Dictionary<string, object>? data);
    Task DeleteBusinessProcess(string name);
}

public class BusinessProcessDetailsViewModel
{
    public string Content { get; set; }

    public List<BusinessProcessElementViewModel> BusinessProcessElementsStatus { get; set; }
}

public class BusinessProcessInstanceDetailsViewModel
{
    public string Id { get; set; }
    public BusinessProcessStatus Status { get; set; }
    public List<BusinessProcessElementViewModel> BusinessProcessElementsStatus { get; set; }
    public DateTime InstantiatedAt { get; internal set; }
}

public class BusinessProcessElementViewModel
{
    public string Id { get; set; }
    public BusinessProcessElementStatus Status { get; set; }
    public bool HasCompensate { get; internal set; }
}
public class BusinessProcessLogViewModel
{
    public DateTime DateTime { get; set; }
    public string ElementId { get; set; }
    public string ElementName { get; set; }
    public string Message { get; set; }
    public BusinessProcessElementStatus Status { get; set; }
}

public class TaskExecutionDescriptionViewModel
{
    public string TaskId { get; set; }
    public TaskExecutorType Type { get; set; }
    public string Url { get; set; }
    public BusinessProcessElementStatus Status { get; set; }
    public DataProviderType DataProviderType { get; internal set; }
    public string CompensationUrl { get; internal set; }
    public TaskExecutorType CompensationType { get; internal set; }
}