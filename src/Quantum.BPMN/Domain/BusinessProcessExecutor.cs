namespace Reng.BPMN.Domain.Domain;

public class BusinessProcessExecutor : IBusinessProcessExecutor
{
    private readonly IBusinessProcessElementExecutorAbstractFactory _abstractFactory;
    private readonly IBusinessProcessRepository _repository;
    public BusinessProcessExecutor(IBusinessProcessElementExecutorAbstractFactory abstractFactory, IBusinessProcessRepository repository)
    {
        _abstractFactory = abstractFactory;
        _repository = repository;
    }

    public void Execute(BusinessProcessInstance aBusinessProcess)
    {
        aBusinessProcess.GoToInProgressState();
        Execute(aBusinessProcess, aBusinessProcess.GetNextElement());

        if (aBusinessProcess.Status == BusinessProcessStatus.Failed)
        {
            while (aBusinessProcess.CompensateStack.Any())
            {
                var pop = aBusinessProcess.CompensateStack.Peek();
                
                Compensate(aBusinessProcess.Context, pop);
                
                aBusinessProcess.CompensateStack.Pop();
            }
        }

        if (aBusinessProcess.EndEvent.Status == BusinessProcessElementStatus.CompletedSuccessfully)
            aBusinessProcess.GoToCompletedSuccessfullyState();

        Save(aBusinessProcess);
    }

    private void Compensate(BpmnExecutionContext context, IAmABusinessProcessElement pop)
    {
        if (pop is IAmAnStartEvent)
        {
            var executor = GetStartEventExecutor((IAmAnStartEvent)pop);
            executor.Compensate(context, (IAmAnStartEvent)pop);
        }
        if (pop is IAmAnEndEvent)
        {
            var executor = GetEndEventExecutor((IAmAnEndEvent)pop);
            executor.Compensate(context, (IAmAnEndEvent)pop);
        }
        if (pop is IAmAServiceTask)
        {
            var executor = GetServiceTaskExecutor(((IAmAServiceTask)pop).CompensateDescription.Type);
            executor.Compensate(context, (IAmAServiceTask)pop);
        }
    }

    private void Execute(BusinessProcessInstance aBusinessProcessInstance, IAmABusinessProcessElement element)
    {
        if (aBusinessProcessInstance.Finished() || element is null)
            return;

        if (element.Status == BusinessProcessElementStatus.CompletedSuccessfully)
            return;

        if (element is not IAmAGetaway)
        {
            try
            {
                aBusinessProcessInstance.SetNextElement(element);

                Execute((dynamic)element, aBusinessProcessInstance.GetContext());
                
                element.GoToCompletedSuccessfullyState();

                aBusinessProcessInstance.LogExecution(element, "با موفقیت اجرا شد");

                aBusinessProcessInstance.CompensateStack.Push(element);

                Save(aBusinessProcessInstance);

                Execute(aBusinessProcessInstance, element.GetNext());
            }
            catch (Exception e)
            {
                element.GoToFailedState();
                aBusinessProcessInstance.GoToFailedState();

                aBusinessProcessInstance.LogExecution(element, "اجرا با شکست مواجه شد. " + e.Message);

                Save(aBusinessProcessInstance);
            }
        }
        else
        {
            if (((IAmAGetaway)element).CanNotGoFurther())
            {
                return;
            }

            element.GoToCompletedSuccessfullyState();

            aBusinessProcessInstance.LogExecution(element, "با موفقیت اجرا شد");

            Save(aBusinessProcessInstance);

            var outgoings = ((IAmAGetaway)element).GetOutgoingList();
            foreach (var amABusinessProcessElement in outgoings)
            {
                Execute(aBusinessProcessInstance, amABusinessProcessElement);
            }
        }
    }

    private void Save(BusinessProcessInstance aBusinessProcess)
    {
        var save = _repository.Update(aBusinessProcess);
        save.Wait();
    }

    private void Execute(IAmAnEndEvent endEvent, BpmnExecutionContext context)
    {
        var executor = GetEndEventExecutor(endEvent);
        executor.Execute(context, endEvent);
    }
    private void Execute(IAmAParallelGetaway parallelGetaway, BpmnExecutionContext context)
    {
        var executor = GetParallelGetawayExecutor(parallelGetaway);
        executor.Execute(context, parallelGetaway);
    }

    private void Execute(IAmAnStartEvent startEvent, BpmnExecutionContext context)
    {
        var executor = GetStartEventExecutor(startEvent);
        executor.Execute(context, startEvent);

    }

    private void Execute(IAmAServiceTask serviceTask, BpmnExecutionContext context)
    {
        var executor = GetServiceTaskExecutor(serviceTask.TaskExecutorDescription.Type);
        executor.Execute(context, serviceTask);
    }

    private void Execute(IAmAnExclusiveGetaway exclusiveGetaway, BpmnExecutionContext context)
    {
        var executor = GetExclusiveGetawayExecutor(exclusiveGetaway);
        executor.Execute(context, exclusiveGetaway);
    }
    private void Execute(IAmAnInclusiveGetaway inclusiveGetaway, BpmnExecutionContext context)
    {
        var executor = GetInclusiveGetawayExecutor(inclusiveGetaway);
        executor.Execute(context, inclusiveGetaway);
    }

    private IInclusiveGetaway GetInclusiveGetawayExecutor(IAmAnInclusiveGetaway inclusiveGetaway)
        => new InclusiveGetaway();

    private IExclusiveGetawayExecutor GetExclusiveGetawayExecutor(IAmAnExclusiveGetaway exclusiveGetaway)
       => new ExclusiveGetawayExecutor();

    private IServiceTaskExecutor GetServiceTaskExecutor(TaskExecutorType type)
    {
        return _abstractFactory.CreateUserTaskExecutor(type);
    }

    private IAmEndEventExecutor GetEndEventExecutor(IAmAnEndEvent endEvent)
        => _abstractFactory.CreateEndEventExecutor(endEvent);
    private IAmParallelGetawayExecutor GetParallelGetawayExecutor(IAmAParallelGetaway parallelGetaway)
        => _abstractFactory.CreateParallelGetawayExecutor(parallelGetaway);
    private IStartEventExecutor GetStartEventExecutor(IAmAnStartEvent startEvent)
        => _abstractFactory.CreateStartEventExecutor(startEvent);
}