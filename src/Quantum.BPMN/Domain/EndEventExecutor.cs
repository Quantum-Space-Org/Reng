namespace Reng.BPMN.Domain.Domain;

public class EndEventExecutor : IAmEndEventExecutor
{
    public void Compensate(BpmnExecutionContext bpmnExecutionContext, IAmAnEndEvent serviceTask)
    {
 
    }

    public void Execute(BpmnExecutionContext bpmnExecutionContext, IAmAnEndEvent endEvent)
    {

    }
}