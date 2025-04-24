using Reng.BPMN.ACL;

namespace Reng.BPMN.Domain.Domain
{
    public record IAmABoundaryEvent(string Id , string AttachTo,bool HasCompensate)
    {
    }
}