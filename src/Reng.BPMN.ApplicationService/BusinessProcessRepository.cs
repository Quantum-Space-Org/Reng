using Microsoft.EntityFrameworkCore;
using Reng.BPMN.Domain;
using Reng.BPMN.Domain.Domain;

namespace Reng.BPMN.ApplicationService;

public class BusinessProcessRepository : IBusinessProcessRepository
{
    private readonly BusinessProcessDbContext _context;

    public BusinessProcessRepository(BusinessProcessDbContext context) => _context = context;

    public async Task Save(BusinessProcess businessProcess, string bpmnContent)
    {
        await _context.BusinessProcesses.AddAsync(businessProcess);

        var businessProcessContent = new BusinessProcessContent
        {
            BusinessProcess = businessProcess,
            BusinessProcessId = businessProcess.Id,
            Content = bpmnContent
        };

        await _context.BusinessProcessesContent.AddAsync(businessProcessContent);

        await _context.SaveChangesAsync();
    }

    public async Task Save(BusinessProcessInstance businessProcess)
    {
        await _context.BusinessProcesseInstances.AddAsync(businessProcess);

        await _context.SaveChangesAsync();
    }


    public async Task<List<BusinessProcessViewModel>> GetAllBusinessProcesses()
        => await _context.BusinessProcesses.Select(bp => new BusinessProcessViewModel(bp.Id, bp.Name, BusinessProcessStatus.NotStarted)).ToListAsync();

    public Task<BusinessProcess> GetByName(string name)
    {
        return _context.BusinessProcesses.FirstOrDefaultAsync(bp => bp.Name == name);
    }

    public async Task DeleteByName(string name)
    {
        var firstOrDefaultAsync = await _context.BusinessProcesses.FirstOrDefaultAsync(bp => bp.Name == name);
        if (firstOrDefaultAsync != null)
        {
            var any = await _context.BusinessProcesseInstances
                .Where(i => i.BusinessProcessId == firstOrDefaultAsync.Id
                            && 
                            (i.Status == BusinessProcessStatus.InProgress 
                             || i.Status == BusinessProcessStatus.Pending))
                .AnyAsync();
            if (any)
                throw new Exception($"{name} دارای اینستنسهای در حال اجرا می باشد");

            _context.BusinessProcesses.Remove(firstOrDefaultAsync);
            await _context.SaveChangesAsync();
        }
    }

    public Task<BusinessProcessInstance> GetBusinessProcessInstanceById(string id)
    {
        return _context.BusinessProcesseInstances.FirstOrDefaultAsync(bp => bp.Id == id);
    }
    public Task Update(BusinessProcess aBusinessProcess)
    {
        _context.BusinessProcesses.Update(aBusinessProcess);
        return _context.SaveChangesAsync();
    }

    public Task Update(BusinessProcessInstance aBusinessProcess)
    {
        _context.BusinessProcesseInstances.Update(aBusinessProcess);
        return _context.SaveChangesAsync();
    }

    public async Task<string> GetBusinessProcessBpmnContentById(string id)
    {
        var result = await _context.BusinessProcessesContent.FirstOrDefaultAsync(a => a.BusinessProcessId == id);
        if (result == null)
            throw new ObjectNotFoundException("فرآیند بیزنسی با ای دی مورد نظر یافت نشد");

        return result.Content;
    }

    public async Task<List<BusinessProcessInstance>> GetAllInstancesOfBusinessProcess(string id)
    {
        var result = await _context.BusinessProcesseInstances
            .Where(a => a.BusinessProcessId == id).ToListAsync();

        return result;
    }

    public async Task<bool> IsNameUnique(string name)
        => await _context.BusinessProcesses.Where(p => p.Name == name).FirstOrDefaultAsync() == null;
}