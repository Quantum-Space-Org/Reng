namespace Reng.BPMN.Domain.Domain;

public class BusinessProcessElementExecutorAbstractFactory : IBusinessProcessElementExecutorAbstractFactory
{
    public virtual IStartEventExecutor CreateStartEventExecutor(IAmAnStartEvent startEvent)
        => new StartEventExecutor();

    public virtual IAmEndEventExecutor CreateEndEventExecutor(IAmAnEndEvent amAnEndEvent)
        => new EndEventExecutor();

    public IAmParallelGetawayExecutor CreateParallelGetawayExecutor(IAmAParallelGetaway parallelGetaway)
    {
        return new AmAParallelGetaway(this);
    }

    public virtual IServiceTaskExecutor CreateUserTaskExecutor(TaskExecutorType type)
    {
        switch (type)
        {
            case TaskExecutorType.NotSetYet:
                return new NullServiceTaskExecutor();
            case TaskExecutorType.RestApi:
                return new RestApiServiceTaskExecutor();
            default:
                throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}