using Reng.BPMN.Domain;
using Reng.BPMN.Domain.Domain;

namespace Reng.Tests.Helpers;

public class StubBusinessProcessElementExecutorAbstractFactory : BusinessProcessElementExecutorAbstractFactory , IBusinessProcessElementExecutorAbstractFactory
{
    private IServiceTaskExecutor _serviceTaskExecutor;

    public static StubBusinessProcessElementExecutorAbstractFactory WhichWhenCallCrateUserTaskExecutorIExpectToReturn(IServiceTaskExecutor serviceTaskExecutor)
    {
        return new StubBusinessProcessElementExecutorAbstractFactory
        {
            _serviceTaskExecutor = serviceTaskExecutor
        };
    }
    public override IServiceTaskExecutor CreateUserTaskExecutor(TaskExecutorType type)
        => _serviceTaskExecutor;
}