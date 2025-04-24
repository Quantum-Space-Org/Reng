namespace Reng.BPMN.Domain.Domain;

public class StartEventExecutor : IStartEventExecutor
{
    public void Compensate(BpmnExecutionContext context, IAmAnStartEvent pop)
    {
    }

    public void Execute(BpmnExecutionContext bpmnExecutionContext, IAmAnStartEvent startEvent)
    {
        
    }
}