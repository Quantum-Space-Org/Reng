using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reng.BPMN.ApplicationService;
using Reng.BPMN.Domain.Domain;
using Reng.Tests.Helpers;
using Reng.BPMN.ACL;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using Task = System.Threading.Tasks.Task;

namespace Reng.Tests
{
    public class AcceptanceTests
    {
        private string baseAddress = "api/";
        private static TestServer _server;

        [Fact]
        public async Task successfully_create_a_business_process_from_bpmn_file()
        {
            var httpClient = CreateHttpClient();

            var httpResponseMessage = await httpClient.PostAsync($"{baseAddress}myBPMN", CreateStringContent(BpmnSampleFilesHelper.CalculatePayrollBpmnFilePath));

            httpResponseMessage.EnsureSuccessStatusCode();

            var responseMessage = await httpClient.GetAsync(baseAddress);
            responseMessage.EnsureSuccessStatusCode();
            var businessProcessViewModel = JsonConvert.DeserializeObject<List<BusinessProcessViewModel>>(await responseMessage.Content.ReadAsStringAsync());
            businessProcessViewModel.Should().NotBeEmpty();

            await httpClient.PutAsync($"{baseAddress}{businessProcessViewModel.Single().Id}/execute", null);
        }


        [Fact]
        public async Task create_new_instance()
        {
            var httpClient = CreateHttpClient();

            var startEvent = new IAmAnStartEvent("startEvent", "startEvent");
            var endEvent = new IAmAnEndEvent("endEvent", "endEvent");
            var businessProcess = new BusinessProcess("id", "bp"
                , startEvent
                , endEvent
                , new List<IAmAServiceTask>
                                {
                                        new ("CalculatePayroll", "CalculatePayroll"),
                                        new ("CalculateTax", "CalculateTax"),
                                        new ("CalculateInsurance", "CalculateInsurance"),
                                        new ("CalculateLoan", "CalculateLoan"),
                                        new ("CalculatePayroll2", "CalculatePayroll2"),
                                }
                , null
                , null
                , new List<IAmAParallelGetaway>
                {
                        new("1", "1"),
                        new("2", "2")
                }, new List<SequenceFlow>
                {
                        new() { SourceRef = "startEvent", TargetRef = "CalculatePayroll" },

                        new() { SourceRef = "CalculatePayroll", TargetRef = "1" },

                        new() { SourceRef = "1", TargetRef = "CalculateTax" },
                        new() { SourceRef = "1", TargetRef = "CalculateInsurance" },
                        new() { SourceRef = "1", TargetRef = "CalculateLoan" },

                        new() { SourceRef = "CalculateTax", TargetRef = "2" },
                        new() { SourceRef = "CalculateInsurance", TargetRef = "2" },
                        new() { SourceRef = "CalculateLoan", TargetRef = "2" },

                        new() { SourceRef = "2", TargetRef = "CalculatePayroll2" },

                        new() { SourceRef = "CalculatePayroll2", TargetRef = "endEvent" }
                }, null);

            var repository = _server.Services.GetService(typeof(IBusinessProcessRepository)) as IBusinessProcessRepository;
            await repository.Save(businessProcess, "content");

            var httpResponseMessage = await httpClient.PostAsync($"{baseAddress}{businessProcess.Id}/newInstance", null);
            httpResponseMessage.EnsureSuccessStatusCode();

            (await httpResponseMessage.Content.ReadAsStringAsync())
                    .Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task element_status()
        {
            var httpClient = CreateHttpClient();

            var businessProcessId = await CreateBusinessProcessFromBpmnFile(httpClient, BpmnSampleFilesHelper.SampleBpmnFilePath);
            var instanceId = await CreateInstance(httpClient, businessProcessId);

            var responseMessage = await httpClient.PutAsync($"{baseAddress}instances/{instanceId}/execute", new StringContent(JsonConvert.SerializeObject(new Dictionary<string, object> { }), Encoding.UTF8, "application/json"));

            responseMessage.EnsureSuccessStatusCode();

            var httpResponseMessage = await httpClient.GetAsync(baseAddress + "instances/" + instanceId);

            var result = JsonConvert.DeserializeObject<BusinessProcessInstanceDetailsViewModel>(await httpResponseMessage.Content.ReadAsStringAsync());

            result.Status
                .Should()
                .Be(BusinessProcessStatus.CompletedSuccessfully);
        }


        [Fact]
        public async Task create_new_instance_and_execute_it_in_one_post_request()
        {
            var httpClient = CreateHttpClient();

            var businessProcessId = await CreateBusinessProcessFromBpmnFile(httpClient, BpmnSampleFilesHelper.SampleBpmnFilePath);
            
            var responseMessage = await httpClient.PutAsync($"{baseAddress}{businessProcessId}/executeNewInstance", new StringContent(JsonConvert.SerializeObject(new Dictionary<string, object> { }), Encoding.UTF8, "application/json"));

            responseMessage.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<ExecutionBusinessProcessStatus>(await responseMessage.Content.ReadAsStringAsync());

            result.Id
                .Should()
                .NotBeEmpty();
                        
            result.Status
                .Should()
                .Be(BusinessProcessStatus.CompletedSuccessfully);
        }

        [Fact]
        public async Task get_list_of_instances()
        {
            var httpClient = CreateHttpClient();

            var businessProcessId = await CreateBusinessProcessFromBpmnFile(httpClient, BpmnSampleFilesHelper.SampleBpmnFilePath);

            await CreateInstance(httpClient, businessProcessId);
            await CreateInstance(httpClient, businessProcessId);
            await CreateInstance(httpClient, businessProcessId);

            var responseMessage = await httpClient.GetAsync($"{baseAddress}{businessProcessId}/instances");

            responseMessage.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<BusinessProcessInstanceDetailsViewModel>>(await responseMessage.Content.ReadAsStringAsync());

            result.Count
                .Should()
                .Be(3);
        }


        private async Task<string> CreateInstance(HttpClient httpClient, string businessProcessId)
        {
            var response = await httpClient.PostAsync($"{baseAddress}{businessProcessId}/newInstance", null);
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadAsStringAsync());
        }

        private async Task<string> CreateBusinessProcessFromBpmnFile(HttpClient httpClient, string sampleBpmnFilePath)
        {
            await httpClient.PostAsync($"{baseAddress}myBPMN", CreateStringContent(BpmnSampleFilesHelper.SampleBpmnFilePath));

            var responseMessage = await httpClient.GetAsync(baseAddress);

            var businessProcessViewModel = JsonConvert.DeserializeObject<List<BusinessProcessViewModel>>(await responseMessage.Content.ReadAsStringAsync());
            return businessProcessViewModel.Single().Id;
        }

        private static StringContent CreateStringContent(string bpmnFilePath)
        {
            return new StringContent(SerializeObject(bpmnFilePath), Encoding.UTF8, "application/json");
        }

        private static string SerializeObject(string bpmnFilePath)
        {
            return JsonConvert.SerializeObject(new
            {
                bpmnFile = BpmnSampleFilesHelper.ReadFileContentAsString(bpmnFilePath)
            });
        }

        private static HttpClient CreateHttpClient()
        {
            var webBuilder = new WebHostBuilder();

            const string payrollManagementApiAssemblyFullName = "Reng.BPMN.API, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

            webBuilder.UseStartup<AcceptanceTestStartup>()
                .UseSetting(WebHostDefaults.ApplicationKey, payrollManagementApiAssemblyFullName);

            webBuilder.ConfigureServices(collection => collection.AddScoped(
                typeof(IBusinessProcessElementExecutorAbstractFactory)
                , typeof(StubBusinessProcessElementExecutorAbstractFactory)));

            _server = new TestServer(webBuilder);

            var httpClient = _server.CreateClient();
            return httpClient;
        }



        private async Task<BusinessProcessDbContext> CreateDbContextAndMigrateDataBase()
        {

            var options = new DbContextOptionsBuilder<BusinessProcessDbContext>().UseSqlite(CreateInMemoryDatabase()).Options;

            var businessProcessDbContext = new BusinessProcessDbContext(options);

            await businessProcessDbContext.Database.EnsureCreatedAsync();
            return businessProcessDbContext;
        }

        private BusinessProcessRepository CreateRepository(BusinessProcessDbContext context)
        {
            var businessProcessRepository = new BusinessProcessRepository(context);
            return businessProcessRepository;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        private static StringContent CreateStreamContent(object content)
        {
            var serializeObject = JsonConvert.SerializeObject(
                content);
            return new StringContent(serializeObject, Encoding.UTF8, "application/json");
        }
    }
}
