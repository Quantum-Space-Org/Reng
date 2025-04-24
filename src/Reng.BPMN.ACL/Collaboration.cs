namespace Reng.BPMN.ACL;
public partial class Collaboration
{
    [JsonProperty("@id")]
    public string Id { get; set; }
    [JsonProperty("@name")]
    public string Name { get; set; }

    [JsonProperty("bpmn:participant")]
    public Participant Participant { get; set; }
}
