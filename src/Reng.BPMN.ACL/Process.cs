using Newtonsoft.Json.Linq;

namespace Reng.BPMN.ACL;

public class Process
{
    [JsonProperty("@id")]
    public string Id { get; set; }
    [JsonProperty("@name")]
    public string Name { get; set; }

    [JsonProperty("@drools:packageName")]
    public string DroolsPackageName { get; set; }
    [JsonProperty("@drools:version")]
    public string DroolsVersion { get; set; }
    [JsonProperty("@drools:adHoc")]
    public bool DroolsAdHoc { get; set; }

    [JsonProperty("@processType")]
    public string ProcessType { get; set; }
    [JsonProperty("bpmn:sequenceFlow")]
    public dynamic SequenceFlows { get; set; }


    [JsonProperty("bpmn:parallelGateway")]
    public dynamic ParallelGetawayList { get; set; }


    [JsonProperty("bpmn:inclusiveGateway")]
    public dynamic InclusiveGatewayList { get; set; }


    [JsonProperty("bpmn:eventBasedGateway")]
    public dynamic EventBaseGatewayList { get; set; }

    [JsonProperty("bpmn:exclusiveGateway")]
    public dynamic ExclusiveGatewayList { get; set; }

    [JsonProperty("bpmn:startEvent")]
    public Event StartEvent { get; set; }

    [JsonProperty("bpmn:task")]
    public dynamic Tasks { get; set; }


    [JsonProperty("bpmn:serviceTask")]
    public dynamic ServiceTasks { get; set; }


    [JsonProperty("bpmn:boundaryEvent")]
    public dynamic BoundaryEvent { get; set; }


    [JsonProperty("bpmn:userTask")]
    public dynamic UserTasks { get; set; }

    [JsonProperty("bpmn:endEvent")]
    public Event EndEvent { get; set; }

    public List<ServiceTask> GetUserTasks()
    {
        if (UserTasks != null && ((Newtonsoft.Json.Linq.JToken)UserTasks).Type == JTokenType.Array)
            return ((JToken)UserTasks).ToObject(typeof(List<ServiceTask>)) as List<ServiceTask>;

        if (UserTasks == null) return new List<ServiceTask>();

        var task = ((JToken)UserTasks)?.ToObject(typeof(ServiceTask)) as ServiceTask;
        return new List<ServiceTask> { task };
    }

    public List<ServiceTask> GetTasks()
    {
        if (Tasks != null && ((JToken)Tasks).Type == JTokenType.Array)
            return ((JToken)Tasks).ToObject(typeof(List<ServiceTask>)) as List<ServiceTask>;

        if (Tasks == null) return new List<ServiceTask>();

        var task = ((JToken)Tasks)?.ToObject(typeof(ServiceTask)) as ServiceTask;
        return new List<ServiceTask> { task };
    }
    public List<ServiceTask> GetServiceTasks()
    {
        if (ServiceTasks != null && ((JToken)ServiceTasks).Type == JTokenType.Array)
            return ((JToken)ServiceTasks).ToObject(typeof(List<ServiceTask>)) as List<ServiceTask>;

        if (ServiceTasks == null) return new List<ServiceTask>();

        var task = ((JToken)ServiceTasks)?.ToObject(typeof(ServiceTask)) as ServiceTask;
        return new List<ServiceTask> { task };
    }

    public List<BoundaryEvent> GetBoundaryEvents()
    {
        if (BoundaryEvent != null && ((JToken)BoundaryEvent).Type == JTokenType.Array)
            return ((JToken)BoundaryEvent).ToObject(typeof(List<BoundaryEvent>)) as List<BoundaryEvent>;

        if (BoundaryEvent == null) return new List<BoundaryEvent>();

        var boundaryEvent = ((JToken)BoundaryEvent)?.ToObject(typeof(BoundaryEvent)) as BoundaryEvent;
        return new List<BoundaryEvent> { boundaryEvent };
    }

    public List<Getaway> GetInclusiveGatewayList()
    {
        if (InclusiveGatewayList != null && ((JToken)InclusiveGatewayList).Type == JTokenType.Array)
            return ((JToken)InclusiveGatewayList).ToObject(typeof(List<Getaway>)) as List<Getaway>;

        if (InclusiveGatewayList != null)
            return new List<Getaway> { ((JToken)InclusiveGatewayList).ToObject(typeof(Getaway)) as Getaway };

        return new List<Getaway>();
    }

    public List<Getaway> GetExclusiveGatewayList()
    {
        if (ExclusiveGatewayList != null && ((JToken)ExclusiveGatewayList).Type == JTokenType.Array)
            return ((JToken)ExclusiveGatewayList).ToObject<List<Getaway>>();
        if (ExclusiveGatewayList != null)
            return new List<Getaway> { ((JToken)ExclusiveGatewayList)?.ToObject<Getaway>() };
        return new List<Getaway>();

    }
    public List<Getaway> GetParallelGatewayList()
    {
        if (ParallelGetawayList != null && ((JToken)ParallelGetawayList).Type == JTokenType.Array)
            return ((JToken)ParallelGetawayList).ToObject<List<Getaway>>();
        if (ParallelGetawayList != null)
            return new List<Getaway> { ((JToken)ParallelGetawayList)?.ToObject<Getaway>() };
        return new List<Getaway>();

    }

    public List<SequenceFlow> GetSequenceFlows()
    {
        if (SequenceFlows != null && ((JToken)SequenceFlows).Type == JTokenType.Array)
            return ((JToken)SequenceFlows).ToObject<List<SequenceFlow>>();

        if (SequenceFlows != null)
            return new List<SequenceFlow> { ((JToken)SequenceFlows).ToObject<SequenceFlow>() };

        return new List<SequenceFlow>();
    }
}