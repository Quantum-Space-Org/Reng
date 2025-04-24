namespace Reng.BPMN.Domain.Domain;

public interface IBusinessProcessExecutor
{
    void Execute(BusinessProcessInstance _aBusinessProcess);
}