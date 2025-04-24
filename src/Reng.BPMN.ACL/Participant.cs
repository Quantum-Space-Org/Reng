
namespace Reng.BPMN.ACL;
public class Participant
{
    [JsonProperty("@id")]
    public string Id { get; set; }

    [JsonProperty("@name")]
    public string Name { get; set; }

    [JsonProperty("@processRef")]
    public string ProcessRef { get; set; }
}