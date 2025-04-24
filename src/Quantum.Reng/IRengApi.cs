using Reng.BPMN.ApplicationService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quantum.Reng
{
    public interface IRengApi
    {
        Task<string> ExecuteANewInsatnce(string businessProcessName ,  Dictionary<string, string> context);
        Task<string> Execute(string instanceId, Dictionary<string, string> context);
        Task<List<BusinessProcessLogViewModel>> GetExecutionDescription(string instanceId);
        Task Delete(string businessProcessName);
    }
}