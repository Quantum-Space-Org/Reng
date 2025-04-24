using Reng.BPMN.Domain;
using Reng.BPMN.Domain.Domain;

namespace Reng.BPMN.ApplicationService;

public class BusinessProcessContent
{
    public string BusinessProcessId { get; set; }
    public BusinessProcess BusinessProcess { get; set; }
    public string Content { get; set; }
}