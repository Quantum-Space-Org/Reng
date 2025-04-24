namespace Reng.BPMN.ACL;
public class ServiceTask
{
    [JsonProperty("@id")]
    public string Id { get; set; }
    [JsonProperty("@name")]
    public string Name { get; set; }

    [JsonProperty("@sourceRef")]
    public string SourceRef { get; set; }
    [JsonProperty("@targetRef")]
    public string TargetRef { get; set; }

    [JsonProperty("bpmn:outgoing")]
    public dynamic Outgoing { get; set; }

    [JsonProperty("bpmn:incoming")]
    public dynamic Incoming { get; set; }

    [JsonProperty("bpmn:extensionElements")]
    public ExtensionElement ExtensionElements { get; set; }
    [JsonProperty("bpmn:ioSpecification")]
    public UserTaskSpecification Specification { get; set; }
    [JsonProperty("bpmn:dataInputAssociation")]
    public List<DataInputAssociation> DataInputAssociation { get; set; }
}

public class BoundaryEvent
{
    [JsonProperty("@id")]
    public string Id{ get; set; }
    [JsonProperty("@attachedToRef")]
    public string AttachedToRef { get; set; }
    [JsonProperty("bpmn:compensateEventDefinition")]
    public CompensateBoundaryEvent CompensateEvent { get; set; }
}

public class CompensateBoundaryEvent
{
    [JsonProperty("@id")]
    public string Id { get; set; }
}
public class DataInputAssociation
{
    [JsonProperty("bpmn:targetRef")]
    public  string TargetRef { get; set; }
    [JsonProperty("bpmn:assignment")]
    public Assignment Assignment { get; set; }
}

public class Assignment
{
    [JsonProperty("bpmn:from")]

    public Bpmn2from From { get; set; }
    [JsonProperty("bpmn:to")]

    public Bpmn2to To { get; set; }
}

public class Bpmn2to
{
    [JsonProperty("@xsi:type")]
    public string XsiType { get; set; }
    [JsonProperty("#cdata-section")]
    public string cdatasection { get; set; }
}

public class Bpmn2from
{
    [JsonProperty("@xsi:type")]
    public string XsiType { get; set; }
    [JsonProperty("#cdata-section")]
    public string cdatasection{ get; set; }
}

public class UserTaskSpecification
{
    [JsonProperty("bpmn:dataInput")]
    public List<DataInput> Input { get; set; }
    [JsonProperty("bpmn:inputSet")]
    public InputSet InputSet { get; set; }
}

public class InputSet
{
    [JsonProperty("bpmn:dataInputRefs")]
    public List<string> DataInputRefs { get; set; }
}
public class DataInput
{
    [JsonProperty("@id")]
    public string Id { get; set; }
    [JsonProperty("@name")]
    public string Name { get; set; }
    [JsonProperty("@drools:dtype")]
    public string @droolsDtype { get; set; }

    [JsonProperty("@itemSubjectRef")]
    public string @itemSubjectRef { get; set; }
}