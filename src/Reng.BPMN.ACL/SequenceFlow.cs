namespace Reng.BPMN.ACL;

        public class SequenceFlow
        {
            [JsonProperty("@id")]
            public string Id { get; set; }
            [JsonProperty("@name")]
            public string Name { get; set; }

            [JsonProperty("@sourceRef")]
            public string SourceRef { get; set; }
            [JsonProperty("@targetRef")]
            public string TargetRef { get; set; }

            [JsonProperty("bpmn:extensionElements")]
            public ExtensionElement ExtensionElements { get; set; }
        }
