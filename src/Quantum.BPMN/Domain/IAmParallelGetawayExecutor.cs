namespace Reng.BPMN.Domain.Domain;

public interface IAmParallelGetawayExecutor
{
    void Execute(BpmnExecutionContext context, IAmAParallelGetaway parallelGetaway);
}