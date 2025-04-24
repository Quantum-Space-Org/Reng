namespace Reng.BPMN.Domain.Domain;

public class AmAParallelGetaway : IAmParallelGetawayExecutor
{
    private readonly IBusinessProcessElementExecutorAbstractFactory _abstractFactory;

    public AmAParallelGetaway(IBusinessProcessElementExecutorAbstractFactory abstractFactory)
    {
        _abstractFactory = abstractFactory;
    }

    public void Execute(BpmnExecutionContext context, IAmAParallelGetaway parallelGetaway)
    {

        var outgoingList = parallelGetaway.OutgoingList;

        foreach (var outgoing in outgoingList)
        {
            Execute(context, outgoing);
        }
    }

    private void Execute(BpmnExecutionContext context, IAmABusinessProcessElement outgoing)

    {
        _abstractFactory.CreateUserTaskExecutor(((IAmAServiceTask)outgoing).TaskExecutorDescription.Type)
            .Execute(context, (IAmAServiceTask)outgoing);
    }
}