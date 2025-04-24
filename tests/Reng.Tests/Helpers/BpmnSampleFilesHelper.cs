using System.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace Reng.Tests.Helpers
{
    internal class BpmnSampleFilesHelper
    {
        public static  string ReserveBpmnFilePath = "\\reserve.bpmn";
        public static string OrderPastaBpmnFilePath = "\\order-pasta.bpmn";
        public const string SampleBpmnFilePath = "\\sample.bpmn";
        public const string CompensateFilePath = "\\compensate.bpmn";
        public const string CalculatePayrollBpmnFilePath = "\\payroll-calculation.bpmn";

        public static string LoadBpmFrom(string bpmnPath)
        {
            var xmlBpmn = ReadFileContentAsString(bpmnPath);

            var doc = new XmlDocument();
            doc.LoadXml(xmlBpmn);

            var json = JsonConvert.SerializeXmlNode(doc, Formatting.Indented);
            return json;
        }

        public static string ReadFileContentAsString(string bpmnPath = "\\sample.bpmn")
        {
            return File.ReadAllText(GetTestPath(bpmnPath));
        }

        private static string GetTestPath(string relativePath)
        {
            var codeBaseUrl = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.FullName + relativePath;
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl);
            return codeBasePath;
        }
    }
}
