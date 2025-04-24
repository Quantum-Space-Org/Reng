namespace Reng.BPMN.ACL
{
    public class Bpmn2Definitions
    {
        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("@xmlns:bpmn2")]
        public string XmlnsBpmn2 { get; set; }

        [JsonProperty("@xmlns:bpmndi")]
        public string XmlnsBpmndi { get; set; }

        [JsonProperty("@xmlns:bpsim")]
        public string XmlnsBpsim { get; set; }


        [JsonProperty("@xmlns:dc")]
        public string XmlnsDc { get; set; }

        [JsonProperty("@xmlns:di")]
        public string XmlnsDi { get; set; }


        [JsonProperty("@xmlns:drools")]
        public string XmlnsDrools { get; set; }

        
        [JsonProperty("@xsi:schemaLocation")]
        public string SchemaLocation { get; set; }
        [JsonProperty("@exporter")]
        public string Exporter { get; set; }
        [JsonProperty("@exporterVersion")]
        public string ExporterVersion { get; set; }

        [JsonProperty("@targetNamespace")]
        public string TargetNamespace { get; set; }

        [JsonProperty("bpmn:collaboration")]
        public Collaboration Collaboration { get; set; }

        [JsonProperty("bpmn:process")]
        public Process Process { get; set; }
    }
}
