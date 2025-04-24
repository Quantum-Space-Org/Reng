namespace Reng.BPMN.Domain.Domain;

public interface IBusinessProcessRepository
{
    Task Save(BusinessProcess businessProcess, string bpmnContent);
    Task Save(BusinessProcessInstance businessProcess);
    Task<List<BusinessProcessViewModel>> GetAllBusinessProcesses();
    Task<BusinessProcess> GetByName(string id);
    Task DeleteByName(string name);
    Task<BusinessProcessInstance> GetBusinessProcessInstanceById(string id);
    Task Update(BusinessProcess aBusinessProcess);
    Task Update(BusinessProcessInstance aBusinessProcess);
    Task<string> GetBusinessProcessBpmnContentById(string id);
    Task<List<BusinessProcessInstance>> GetAllInstancesOfBusinessProcess(string id);
    Task<bool> IsNameUnique(string name);
    
}