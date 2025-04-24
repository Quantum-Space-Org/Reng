using Reng.BPMN.Domain.Domain;

namespace Reng.BPMN.ApplicationService
{
    public class ExecutionBusinessProcessStatus
    {
        public string Id { get;  set; }
        public BusinessProcessStatus Status { get; set; }
    }
}