namespace Reng.BPMN.ACL;


public class Getaway
{
    [JsonProperty("@id")]
    public string Id { get; set; }
    [JsonProperty("@name")]
    public string Name { get; set; }
    [JsonProperty("@gatewayDirection")]
    public string GatewayDirection { get; set; }
    [JsonProperty("bpmn:incoming")]
    public dynamic Incoming { get; set; }
    [JsonProperty("bpmn:outgoing")]
    public dynamic Outgoings { get; set; }
    [JsonProperty("bpmn:extensionElements")]
    public extensionElements ExtensionElements { get; set; }
    public class extensionElements
    {
        [JsonProperty("@id")]
        public string Id { get; set; }
        [JsonProperty("@name")]
        public string Name { get; set; }

        [JsonProperty("drools:metaData")]
        public Drools Drolls { get; set; }
    }

}
