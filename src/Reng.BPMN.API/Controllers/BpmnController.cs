using Microsoft.AspNetCore.Mvc;
using Reng.BPMN.ApplicationService;
using Reng.BPMN.Domain.Domain;

namespace Reng.BPMN.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class BpmnController : ControllerBase
    {
        private readonly ILogger<BpmnController> _logger;
        private readonly IBpmnApplicationService _applicationService;
        public BpmnController(ILogger<BpmnController> logger, IBpmnApplicationService applicationService)
        {
            _logger = logger;
            _applicationService = applicationService;
        }

        [HttpPost("{name}")]
        public async Task<IActionResult> Put(string name, [FromBody] CreateBusinessProcessDto dto)
        {
            var businessUnitResult = await _applicationService.CreateBusinessProcess(name, dto.BpmnFile);
            return base.Created($"/api/{businessUnitResult.id}", businessUnitResult);
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete(string name)
        {
            await _applicationService.DeleteBusinessProcess(name);
            return Ok();
        }

        [HttpGet]
        public async Task<List<BusinessProcessViewModel>> Get()
            => await _applicationService.GetBusinessProcessList();

        [HttpGet("{name}")]
        public async Task<BusinessProcessDetailsViewModel> Get(string name)
            => await _applicationService.GetBusinessProcess(name);

        [HttpGet("{name}/instances")]
        public async Task<List<BusinessProcessInstanceDetailsViewModel>> GetInstancesOf(string name)
            => await _applicationService.GetBusinessProcessInstances(name);

        [HttpGet("instances/{instanceId}")]
        public async Task<BusinessProcessInstanceDetailsViewModel> GetInstance(string instanceId)
           => await _applicationService.GetBusinessProcessInstance(instanceId);

        [HttpGet("instances/{instanceId}/logs")]
        public async Task<List<BusinessProcessLogViewModel>> GetLogs(string instanceId)
            => await _applicationService.GetBusinessProcessLogs(instanceId);

        [HttpGet("{name}/Task/{taskId}")]
        public async Task<TaskExecutionDescriptionViewModel> GetContent(string name, string taskId)
            => await _applicationService.GetTaskDescription(name, taskId);

        [HttpPut("{name}/Task/{taskId}/executionDescription")]
        public async Task GetContent(string name, string taskId, [FromBody] TaskExecutorDescription description)
            => await _applicationService.AddTaskDescription(name, taskId, description);

        [HttpPut("{name}/Task/{taskId}/inputDataProvider")]
        public async Task SetInputDataProvider(string name, string taskId, DataProviderType type)
            => await _applicationService.AddTaskDataProvider(name, taskId, type);

        [HttpPut("{name}/Task/{taskId}/compensation")]
        public async Task SetCompensation(string name, string taskId, [FromBody] CompensateExecution compensateExecution)
            => await _applicationService.AddTaskCompensation(name, taskId, compensateExecution);

        [HttpPut("instances/{id}/execute")]
        public async Task<ExecutionBusinessProcessStatus> Execute(string id, [FromBody] Dictionary<string, object>? data = null)
            => await _applicationService.ExecuteBusinessProcess(id, data);

        [HttpPut("{name}/executeNewInstance")]
        public async Task<ExecutionBusinessProcessStatus> ExecuteNewInstance(string name, [FromBody] Dictionary<string, object>? data = null)
            => await _applicationService.ExecuteNewInstanceOfBusinessProcess(name, data);

        [HttpPost("{name}/newInstance")]
        public async Task<IActionResult> CreateNewInstance(string name)
        {
            var instanceId = await _applicationService.CreateInstanceFromBusinessProcess(name);
            return Ok(instanceId);
        }



        private static string GetTestPath(string relativePath)
        {
            var codeBaseUrl = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.FullName + relativePath;
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl);
            return codeBasePath;
        }

        [HttpGet("besaz")]
        public async Task<CreatedResult> hh()
        {
            _logger.LogInformation("Misazam");

            var file = System.IO.File.ReadAllText(GetTestPath("\\payroll-calculation.bpmn"));


            var businessUnitResult = await _applicationService.CreateBusinessProcess("myBusinessProcess", file);

            return base.Created($"/api/{businessUnitResult.id}", businessUnitResult);
        }

    }
}