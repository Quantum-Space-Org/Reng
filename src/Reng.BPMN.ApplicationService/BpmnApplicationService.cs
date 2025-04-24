using System;
using System.Xml;
using Newtonsoft.Json;
using Reng.BPMN.Domain;
using Reng.BPMN.Domain.Domain;
using Formatting = Newtonsoft.Json.Formatting;

namespace Reng.BPMN.ApplicationService;

public class BpmnApplicationService : IBpmnApplicationService
{
    private readonly IBusinessProcessRepository _repository;
    private readonly IBusinessProcessElementExecutorAbstractFactory _factory;

    public BpmnApplicationService(IBusinessProcessRepository repository, IBusinessProcessElementExecutorAbstractFactory factory)
    {
        _repository = repository;
        _factory = factory;
    }

    public async Task<BusinessUnitResult> CreateBusinessProcess(string name, string bpmnContent)
    {
        await AssertThatNameIsUnique(name);
        var stream = new StringReader(bpmnContent);

        var doc = new XmlDocument();
        doc.Load(stream);

        var json = JsonConvert.SerializeXmlNode(doc, Formatting.Indented);


        var bpmn2 = JsonConvert.DeserializeObject<ACL.Bpmn2>(json);

        var businessProcess = BusinessProcessFactory.Create(name, bpmn2);

        await _repository.Save(businessProcess, bpmnContent);
        return new BusinessUnitResult(businessProcess.Id, businessProcess.Name);
    }

    private async Task AssertThatNameIsUnique(string name)
    {
        var isNameUnique = await _repository.IsNameUnique(name);
        if (!isNameUnique)
            throw new RengDomainException("نام نمی تواند تکراری باشد");
    }

    public async Task<List<Domain.Domain.BusinessProcessViewModel>> GetBusinessProcessList()
    {
        return await _repository.GetAllBusinessProcesses();
    }


    public async Task<ExecutionBusinessProcessStatus> ExecuteBusinessProcess(string id, Dictionary<string, object>? data)
    {
        var instance = await _repository.GetBusinessProcessInstanceById(id);
        GuardAgainstNotNullBusinessProcess(instance);
        instance.FillContextWith(data);

        var executor = new BusinessProcessExecutor(_factory, _repository);

        executor.Execute(instance);

        return new ExecutionBusinessProcessStatus
        {
            Id = instance.Id,
            Status = instance.Status
        };
    }

    public async Task AddTaskDescription(string name, string taskId, TaskExecutorDescription taskExecutorDescription)
    {
        var businessProcess = await GetByName(name);

        businessProcess.AddTaskDescription(taskId, taskExecutorDescription);

        await _repository.Update(businessProcess);
    }

    public async Task AddTaskCompensation(string name, string taskId, CompensateExecution compensateExecution)
    {
        var businessProcess = await GetByName(name);

        businessProcess.AddTaskCompensation(taskId, compensateExecution);

        await _repository.Update(businessProcess);
    }
    public async Task AddTaskDataProvider(string name, string taskId, DataProviderType type)
    {
        var businessProcess = await GetByName(name);

        businessProcess.AddTaskDataProvider(taskId, type);

        await _repository.Update(businessProcess);
    }

    private async Task<BusinessProcess> GetByName(string name)
    {
        var businessProcess = await _repository.GetByName(name);
        GuardAgainstNotNullBusinessProcess(businessProcess);

        return businessProcess;
    }

    public async Task<TaskExecutionDescriptionViewModel> GetTaskDescription(string name, string taskId)
    {
        var businessProcess = await GetByName(name);

        var aUserTask = businessProcess.GetTaskDescription(taskId);

        return new TaskExecutionDescriptionViewModel
        {
            TaskId = aUserTask.Id,
            Type = aUserTask.TaskExecutorDescription.Type,
            Url = aUserTask.TaskExecutorDescription.Url,
            Status = aUserTask.Status,
            DataProviderType = aUserTask.InputDataProvider.GetType(),
            CompensationUrl = aUserTask.CompensateDescription.Url,
            CompensationType = aUserTask.CompensateDescription.Type,
        };
    }

    private static void GuardAgainstNotNullBusinessProcess(BusinessProcess businessProcess)
    {
        if (businessProcess == null)
            throw new ObjectNotFoundException("فرآیند مورد نظر یافت نشد");
    }
    private static void GuardAgainstNotNullBusinessProcess(BusinessProcessInstance businessProcess)
    {
        if (businessProcess == null)
            throw new ObjectNotFoundException("اینستنس مورد نظر یافت نشد");
    }

    public async Task<List<BusinessProcessLogViewModel>> GetBusinessProcessLogs(string id)
    {
        var businessProcess = await _repository.GetBusinessProcessInstanceById(id);

        return businessProcess.Logs
            .Select(l => new BusinessProcessLogViewModel
            {
                DateTime = l.DateTime,
                ElementId = l.ElementId,
                ElementName = l.ElementName,
                Message = l.Message,
                Status = l.Status
            }).ToList();
    }

    public async Task<BusinessProcessDetailsViewModel> GetBusinessProcess(string name)
    {
        var businessProcess = await _repository.GetByName(name);

        var businessProcessContent 
            = await _repository.GetBusinessProcessBpmnContentById(businessProcess.Id);


        var businessProcessElementStatusMap = new List<BusinessProcessElementViewModel>()
        {
            new()
            {
                Id = businessProcess.StartEvent.Id,
                HasCompensate = businessProcess.StartEvent.HasCompensate
            },
            new()
            {
                Id = businessProcess.EndEvent.Id,
                HasCompensate = businessProcess.EndEvent.HasCompensate
            }
        };

        foreach (var st in businessProcess.ServiceTasks)
        {
            businessProcessElementStatusMap.Add(new BusinessProcessElementViewModel
            {
                Id = st.Id,
                HasCompensate = st.HasCompensate
            });
        }

        foreach (var ig in businessProcess.InclusiveGetaways)
        {
            businessProcessElementStatusMap.Add(new BusinessProcessElementViewModel
            {
                Id = ig.Id
            });
        }

        foreach (var eg in businessProcess.ExclusiveGetaway)
        {
            businessProcessElementStatusMap.Add(new BusinessProcessElementViewModel
            {
                Id = eg.Id
            });
        }

        return new BusinessProcessDetailsViewModel
        {
            Content = businessProcessContent,
            BusinessProcessElementsStatus = businessProcessElementStatusMap
        };
    }

    public async Task<List<BusinessProcessInstanceDetailsViewModel>?> GetBusinessProcessInstances(string name)
    {
        var businessProcess = await _repository.GetByName(name);
        var result = await _repository.GetAllInstancesOfBusinessProcess(businessProcess.Id);
        return result?.Select(ToViewModel).ToList();
    }

    public async Task<BusinessProcessInstanceDetailsViewModel> GetBusinessProcessInstance(string id)
    {
        var businessProcessInstance = await _repository.GetBusinessProcessInstanceById(id);

        return ToViewModel(businessProcessInstance);
    }
    public BusinessProcessInstanceDetailsViewModel ToViewModel(BusinessProcessInstance businessProcessInstance)
    {
        var businessProcessElementStatusMap = new List<BusinessProcessElementViewModel>()
        {
            new()
            {
                Id = businessProcessInstance.StartEvent.Id,
                Status= businessProcessInstance.StartEvent.Status,
                HasCompensate = businessProcessInstance.StartEvent.HasCompensate
            },
            new()
            {
                Id = businessProcessInstance.EndEvent.Id,
                Status =businessProcessInstance.EndEvent.Status,
                HasCompensate = businessProcessInstance.EndEvent.HasCompensate
            }
        };

        foreach (var st in businessProcessInstance.ServiceTasks)
        {
            businessProcessElementStatusMap.Add(new BusinessProcessElementViewModel
            {
                Id = st.Id,
                Status = st.Status,
                HasCompensate = st.HasCompensate
            });
        }

        foreach (var ig in businessProcessInstance.InclusiveGetaways)
        {
            businessProcessElementStatusMap.Add(new BusinessProcessElementViewModel
            {
                Id = ig.Id,
                Status = ig.Status
            });
        }

        foreach (var eg in businessProcessInstance.ExclusiveGetaway)
        {
            businessProcessElementStatusMap.Add(new BusinessProcessElementViewModel
            {
                Id = eg.Id,
                Status = eg.Status
            });
        }

        return new BusinessProcessInstanceDetailsViewModel
        {
            Id = businessProcessInstance.Id,
            InstantiatedAt = businessProcessInstance.InstantiatedAt,
            Status = businessProcessInstance.Status,
            BusinessProcessElementsStatus = businessProcessElementStatusMap
        };
    }

    public async Task<string> CreateInstanceFromBusinessProcess(string name)
    {
        var businessProcess = await GetByName(name);
        BusinessProcessInstance instance = businessProcess.CreateInstance();

        await _repository.Save(instance);

        return instance.Id;
    }

    public async Task<ExecutionBusinessProcessStatus> ExecuteNewInstanceOfBusinessProcess
        (string name, Dictionary<string, object>? data)
    {
        var businessProcess = await GetByName(name);

        var instanceId = await CreateInstanceFromBusinessProcess(businessProcess.Id);

        await ExecuteBusinessProcess(instanceId, data);

        var instance = await GetBusinessProcessInstance(instanceId);
        return new ExecutionBusinessProcessStatus
        {
            Id = instanceId,
            Status = instance.Status
        };
    }

    public async Task DeleteBusinessProcess(string name)
    {
        await _repository.DeleteByName(name);
    }
}

public record BusinessUnitResult(string id, string name);