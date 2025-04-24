namespace Reng.BPMN.Domain.Domain;

public interface IAmEndEventExecutor
{
    void Execute(BpmnExecutionContext bpmnExecutionContext, IAmAnEndEvent endEvent);
    void Compensate(BpmnExecutionContext bpmnExecutionContext, IAmAnEndEvent serviceTask);
}