namespace Reng.BPMN.Domain.Domain;

public interface IStartEventExecutor
{
    void Execute(BpmnExecutionContext bpmnExecutionContext, IAmAnStartEvent startEvent);
    void Compensate(BpmnExecutionContext context, IAmAnStartEvent pop);
}