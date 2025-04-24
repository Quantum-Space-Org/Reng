namespace Reng.BPMN.ACL
{
    public class Bpmn2XmlHeader
    {
        [JsonProperty("@version")]
        public string Version { get; set; }
        [JsonProperty("@encoding")]
        public string Endocing { get; set; }
    }

}
