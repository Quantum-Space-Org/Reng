namespace Reng.BPMN.Domain.Domain;

public interface IServiceTaskExecutor
{
    void Execute(BpmnExecutionContext context, IAmAServiceTask serviceTask);
    void Compensate(BpmnExecutionContext bpmnExecutionContext, IAmAServiceTask serviceTask);
}