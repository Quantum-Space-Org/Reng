using Reng.BPMN.ACL;

namespace Reng.BPMN.Domain.Domain;

public static class BusinessProcessFactory
{
    public static BusinessProcess Create(string name, Bpmn2 bpmn2)
    {
        var process = bpmn2.GetProcess();

        var processStartEvent = process.StartEvent;

        var inclusiveGatewayList = process.GetInclusiveGatewayList();
        var exclusiveGatewayList = process.GetExclusiveGatewayList();
        var parallelGatewayList = process.GetParallelGatewayList();

        var sequenceFlows = bpmn2.Bpmn2Definitions.Process.GetSequenceFlows();

        var startAnAnEvent = ToStartEvent(processStartEvent);
        var amAnEndEvent = ToEndEvent(process.EndEvent);

        var serviceTasks = ToUserTasks(process.GetServiceTasks());

        var boundaryEvents = ToBoundaryEvents(process.GetBoundaryEvents());

        var amAnInclusiveGetaways = ToInclusiveGetawayList(inclusiveGatewayList);

        var amAnExclusiveGetaways = ToExclusiveGetawayList(exclusiveGatewayList);

        var parallelGetaways = ToParallelGetawayList(parallelGatewayList);

        return new(Guid.NewGuid().ToString(), name, startAnAnEvent
                                , amAnEndEvent
                                , serviceTasks
                                , amAnInclusiveGetaways
                                , amAnExclusiveGetaways
                                , parallelGetaways
                                , sequenceFlows: sequenceFlows
                                , boundaryEvents : boundaryEvents);
    }

    private static List<IAmABoundaryEvent> ToBoundaryEvents(List<BoundaryEvent> boundaryEvents)
    {
        return
            boundaryEvents?.Select(be => new IAmABoundaryEvent(be.Id, be.AttachedToRef, be.CompensateEvent != null)).ToList();
    }

    private static List<IAmAnInclusiveGetaway> ToInclusiveGetawayList(IEnumerable<Getaway> inclusiveGatewayList)
    {
        return inclusiveGatewayList?.Select(ig => new IAmAnInclusiveGetaway(ig.Id, ig.Name)).ToList();
    }

    private static List<IAmAnExclusiveGetaway> ToExclusiveGetawayList(List<Getaway> exclusiveGatewayList)
        => exclusiveGatewayList?.Select(g => new IAmAnExclusiveGetaway(g.Id, g.Name)).ToList();

    private static List<IAmAParallelGetaway> ToParallelGetawayList(List<Getaway> exclusiveGatewayList)
        => exclusiveGatewayList?.Select(g => new IAmAParallelGetaway(g.Id, g.Name)).ToList();

    private static List<IAmAServiceTask> ToUserTasks(IEnumerable<ServiceTask> userTasks)
    {
        return userTasks != null && userTasks.Any()
            ? userTasks.Select(ut => new IAmAServiceTask(ut.Id, ut.Name)).ToList()
                : new List<IAmAServiceTask>();
    }

    private static IAmAnStartEvent ToStartEvent(Event processStartEvent)
        => new IAmAnStartEvent(processStartEvent.Id, processStartEvent.Name);

    private static IAmAnEndEvent ToEndEvent(Event endEvent)
        => new IAmAnEndEvent(endEvent.Id, endEvent.Name);
}