namespace Reng.BPMN.ACL;

        public class Drools
        {
            [JsonProperty("@name")]
            public string Name { get; set; }

            [JsonProperty("drools:metaValue")]
            public DroolsValue Value { get; set; }
            public class DroolsValue
            {
                [JsonProperty("#cdata-section")]
                public string cdatasection { get; set; }
            }
        }