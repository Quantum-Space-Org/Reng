namespace Reng.BPMN.Domain.Domain;

internal class ExecutionContextDataProvider : IDataProvider
{
    private readonly BpmnExecutionContext _bpmnExecutionContext;

    public ExecutionContextDataProvider(BpmnExecutionContext bpmnExecutionContext)
    {
        _bpmnExecutionContext = bpmnExecutionContext;
    }

    public object Get(string name)
    {
        return _bpmnExecutionContext.Get(name);
    }
}