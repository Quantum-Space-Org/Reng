namespace Reng.BPMN.Domain.Domain;

internal interface IInclusiveGetaway
{
    void Execute(BpmnExecutionContext getContext, IAmAnInclusiveGetaway inclusiveGetaway);
}