namespace Reng.BPMN.Domain.Domain;

public interface IBusinessProcessElementExecutorAbstractFactory
{
    IStartEventExecutor CreateStartEventExecutor(IAmAnStartEvent startEvent);
    IAmEndEventExecutor CreateEndEventExecutor(IAmAnEndEvent amAnEndEvent);
    IAmParallelGetawayExecutor CreateParallelGetawayExecutor(IAmAParallelGetaway parallelGetaway);
    IServiceTaskExecutor CreateUserTaskExecutor(TaskExecutorType type);
}