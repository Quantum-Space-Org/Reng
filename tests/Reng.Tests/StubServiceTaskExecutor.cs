using FluentAssertions;
using Reng.BPMN.Domain.Domain;

namespace Reng.Tests
{
    internal class StubServiceTaskExecutor : IServiceTaskExecutor
    {
        private int _numberOfCompensateCalled;

        internal static IServiceTaskExecutor New()
        {
            return new StubServiceTaskExecutor(); 
        }

        public void Compensate(BpmnExecutionContext bpmnExecutionContext, IAmAServiceTask serviceTask)
        {
            _numberOfCompensateCalled++;
        }

        public void Execute(BpmnExecutionContext context, IAmAServiceTask serviceTask)
        {
            if (serviceTask.Name == "CalculatePayroll2")
                throw new Exception();
        }

        public void VerifyThatNumberOfCalledIs(int numberOfCalled)
        {
            _numberOfCompensateCalled.Should().Be(numberOfCalled);
        }
    }
}