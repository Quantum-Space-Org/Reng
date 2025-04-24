namespace Reng.BPMN.ACL
{
    public class Bpmn2
    {

        [JsonProperty("?xml")]
        public Bpmn2XmlHeader Bpmn2XmlHeader { get; set; }

        [JsonProperty("bpmn:definitions")]
        public Bpmn2Definitions Bpmn2Definitions { get; set; }

        public Process GetProcess()
        {
            return Bpmn2Definitions.Process;
        }
    }
}