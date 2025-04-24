namespace Reng.BPMN.Domain.Domain;

public class NullServiceTaskExecutor : IServiceTaskExecutor
{
    public void Execute(BpmnExecutionContext context, IAmAServiceTask serviceTask)
    {

    }

    public void Compensate(BpmnExecutionContext bpmnExecutionContext, IAmAServiceTask serviceTask)
    {
        
    }
}