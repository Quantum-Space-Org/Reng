namespace Reng.BPMN.Domain.Domain;

internal interface IExclusiveGetawayExecutor
{
    void Execute(BpmnExecutionContext getContext, IAmAnExclusiveGetaway exclusiveGetaway);
}