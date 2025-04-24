using System.Data.Common;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Reng.BPMN.ACL;
using Reng.BPMN.ApplicationService;
using Reng.BPMN.Domain;
using Reng.BPMN.Domain.Domain;
using Reng.Tests.Helpers;
using Task = System.Threading.Tasks.Task;

namespace Reng.Tests
{
    public class Bpmn2Tests
    {
        private string _reserveBpmnFilePath = "\\reserve.bpmn";
        private string _orderPastaBpmnFilePath = "\\sample.bpmn";
        private string _canvasBpmnFilePath = "\\canvas.bpmn";

        [Fact]
        public void canvas()
        {
            var json = BpmnSampleFilesHelper.LoadBpmFrom(_canvasBpmnFilePath);
            var xyz = JsonConvert.DeserializeObject<Bpmn2>(json);
        }

        [Fact]
        public void test()
        {
            var json = BpmnSampleFilesHelper.LoadBpmFrom(_reserveBpmnFilePath);
            var xyz = JsonConvert.DeserializeObject<Bpmn2>(json);

            //var tasks =
            //    ((Newtonsoft.Json.Linq.JToken)
            //        xyz
            //            .Bpmn2Definitions
            //            .Process
            //            .ExclusiveGatewayList).ToObject(typeof(Getaway));
        }

        [Fact]
        public void compensate_boudary_event()
        {
            var json = BpmnSampleFilesHelper.LoadBpmFrom(BpmnSampleFilesHelper.CompensateFilePath);
            var bpmn2 = JsonConvert.DeserializeObject<Bpmn2>(json);
            var businessProcess = BusinessProcessFactory.Create("name", bpmn2);

            businessProcess.ServiceTasks.Single()
                .HasCompensate
                .Should().BeTrue();
        }

        [Fact]
        public void start_event_should_not_have_incoming()
        {
            var anAnStartEvent = new IAmAnStartEvent("id", "name");

            anAnStartEvent.Status.Should().Be(BusinessProcessElementStatus.NotStarted);

            var action = () => anAnStartEvent.SetIncoming(new IAmAServiceTask("id", "name"));
            action.Should().Throw<AddIncomingToStartEventException>();
        }

        [Fact]
        public void start_event_should_not_have_more_than_one_outgoing()
        {
            // Arrange
            var anAnStartEvent = new IAmAnStartEvent("id", "name");
            anAnStartEvent.SetOutgoing(new IAmAServiceTask("id", "name"));

            // Act
            var action = () => anAnStartEvent.SetOutgoing(new IAmAServiceTask("id", "name"));

            // Assert
            action.Should().Throw<AddMoreThanOneOutgoingToStartEventException>();
        }

        [Fact]
        public void end_event_should_not_have_more_than_one_outgoing()
        {
            // Arrange
            var anAnStartEvent = new IAmAnEndEvent("id", "name");
            anAnStartEvent.SetOutgoing(new IAmAServiceTask("id", "name"));
            // Act
            var action = () => anAnStartEvent.SetOutgoing(new IAmAServiceTask("id", "name"));
            // Assert
            action.Should().Throw<AddMoreThanOneOutgoingToEndEventException>();
        }

        [Fact]
        public async Task log_execution_progress()
        {
            var startEvent = new IAmAnStartEvent("start", "start");

            var aServiceTask = new IAmAServiceTask("service", "service");

            var endEvent = new IAmAnEndEvent("end", "end");

            var businessProcess = new BusinessProcess("id"
                                                        , "my bp"
                                                        , startEvent
                                                        , endEvent
                                                        , new List<IAmAServiceTask> { aServiceTask }
                                                        , null
                                                        , null
                                                        , null
                                                        , new List<SequenceFlow>
                                                        {
                                                                new()
                                                                {
                                                                    SourceRef = startEvent.Id,
                                                                    TargetRef = aServiceTask.Id
                                                                },
                                                                new()
                                                                {
                                                                    SourceRef = aServiceTask.Id,
                                                                    TargetRef = endEvent.Id
                                                                }
                                                        }, null);

            var businessProcessRepository = CreateRepository(CreateDbContextAndMigrateDataBase().Result);

            await businessProcessRepository.Save(businessProcess, "content");

            var instance = businessProcess.CreateInstance();
            await businessProcessRepository.Save(instance);

            var executor = new BusinessProcessExecutor(new BusinessProcessElementExecutorAbstractFactory(), businessProcessRepository);

            executor.Execute(instance);

            var process = await businessProcessRepository.GetBusinessProcessInstanceById(instance.Id);
            process.Logs
                .Count
                .Should()
                .Be(3);
        }

        [Fact]
        public async Task handle_exception()
        {
            var startEvent = new IAmAnStartEvent("start", "start");

            var aServiceTask = new IAmAServiceTask("service", "service")
            {
            };
            aServiceTask.SetExecutorDescription(new TaskExecutorDescription
            {

                Type = TaskExecutorType.RestApi,
                Url = "http"
            });

            var endEvent = new IAmAnEndEvent("end", "end");

            var businessProcess = new BusinessProcess("id"
                                                        , "my bp"
                                                        , startEvent
                                                        , endEvent
                                                        , new List<IAmAServiceTask> { aServiceTask }
                                                        , null
                                                        , null
                                                        , null
                                                        , new List<SequenceFlow>
                                                        {
                                                                new()
                                                                {
                                                                    SourceRef = startEvent.Id,
                                                                    TargetRef = aServiceTask.Id
                                                                },
                                                                new()
                                                                {
                                                                    SourceRef = aServiceTask.Id,
                                                                    TargetRef = endEvent.Id
                                                                }
                                                        }, null);

            var businessProcessRepository = CreateRepository(CreateDbContextAndMigrateDataBase().Result);

            await businessProcessRepository.Save(businessProcess, "content");
            var instance = businessProcess.CreateInstance();
            await businessProcessRepository.Save(instance);

            var executor = new BusinessProcessExecutor(new BusinessProcessElementExecutorAbstractFactory(), businessProcessRepository);

            executor.Execute(instance);

            var process = await businessProcessRepository.GetBusinessProcessInstanceById(instance.Id);
            process.Status.Should().Be(BusinessProcessStatus.Failed);

            process
            .StartEvent
            .Status
            .Should().Be(BusinessProcessElementStatus.CompletedSuccessfully);

            process
                .ServiceTasks
                .Single()
                .Status
                .Should().Be(BusinessProcessElementStatus.Failed);

            process
                .EndEvent
                .Status
                .Should().Be(BusinessProcessElementStatus.NotStarted);
        }

        [Fact]
        public void user_task_should_not_have_more_than_one_outgoing()
        {
            // Arrange
            var aServiceTask = new IAmAServiceTask("id", "name");
            aServiceTask.SetOutgoing(new IAmAServiceTask("id", "name"));

            // Act
            var action = () => aServiceTask.SetOutgoing(new IAmAServiceTask("id", "name"));

            // Assert
            action.Should().Throw<AddMoreThanOneOutgoingToUserTaskException>();
        }

        [Fact]
        public async Task set_task_description()
        {
            IBpmnApplicationService service = CreateBpmnApplicationService();
            await service.CreateBusinessProcess("my-business-process",
                BpmnSampleFilesHelper.ReadFileContentAsString());
            var businessProcessViewModels = await service.GetBusinessProcessList();

            var taskId = "Activity_09yoao6";

            await service.AddTaskDescription(businessProcessViewModels.First().Name, taskId: taskId, new TaskExecutorDescription
            {
                Type = TaskExecutorType.RestApi,
                Url = "localhost"
            });

            var viewModel = await service.GetTaskDescription(id: businessProcessViewModels.First().Name, taskId: taskId);

            viewModel.Should().NotBeNull();
            viewModel.Url.Should().BeEquivalentTo("localhost");
            viewModel.Type.Should().Be(TaskExecutorType.RestApi);
        }

        [Fact]
        public async Task set_task_data_type_provider()
        {
            IBpmnApplicationService service = CreateBpmnApplicationService();
            await service.CreateBusinessProcess("my-business-process", BpmnSampleFilesHelper.ReadFileContentAsString());
            var businessProcessViewModels
                = await service.GetBusinessProcessList();

            var taskId = "Activity_09yoao6";

            await service.AddTaskDataProvider(id: businessProcessViewModels.First().Name, taskId: taskId, DataProviderType.ExecutionContext);

            var viewModel = await service.GetTaskDescription(id: businessProcessViewModels.First().Name, taskId: taskId);

            viewModel.Should().NotBeNull();
            viewModel.DataProviderType.Should().Be(DataProviderType.ExecutionContext);
        }

        [Fact]
        public async Task get_business_process_bpmn_file()
        {
            IBpmnApplicationService service = CreateBpmnApplicationService();
            await service.CreateBusinessProcess("my-business-process", BpmnSampleFilesHelper.ReadFileContentAsString());
            var businessProcessViewModels
                = await service.GetBusinessProcessList();

            var content
                = await service.GetBusinessProcess(businessProcessViewModels.First().Name);

            content.Content.Should().NotBeNull();
        }

        [Fact]
        public void out_put_data()
        {
            // Arrange
            var outputData = new OutputData();

            // Act
            outputData.Set("id", "123", FieldType.String);

            // Assert
            outputData.Get("id").Should().Be("123");
        }

        [Fact]
        public void define_manual_data_provider()
        {
            // Arrange
            var manualDataProvider = new ManualDataProvider();

            // Act
            manualDataProvider.SetProperty(name: "id", type: FieldType.String, "123");

            // Assert
            manualDataProvider
                .GetProperty("id").Should()
                .BeEquivalentTo("123");

            // Act
            manualDataProvider.SetProperty(name: "name", type: FieldType.String, "Martin");

            // Assert
            manualDataProvider
                .GetProperty("name")
                .Should().BeEquivalentTo("Martin");
        }

        [Fact]
        public async Task successfully_save_business_process()
        {
            IBpmnApplicationService service = CreateBpmnApplicationService();

            await service.CreateBusinessProcess(name: "my business process", bpmnContent: BpmnSampleFilesHelper.ReadFileContentAsString());

            var businessProcessViewModels = await service.GetBusinessProcessList();

            businessProcessViewModels.Count.Should().Be(1);
            businessProcessViewModels.Single().Name.Should().BeEquivalentTo("my business process");
            businessProcessViewModels.Single().Status.Should().Be(BusinessProcessStatus.NotStarted);
        }

        [Fact]
        public async Task save_business_process_between_execution()
        {
            var repository = CreateRepository(await CreateDbContextAndMigrateDataBase());
            var service = new BpmnApplicationService(repository, StubBusinessProcessElementExecutorAbstractFactory.WhichWhenCallCrateUserTaskExecutorIExpectToReturn(new NullServiceTaskExecutor()));

            await service.CreateBusinessProcess(name: "my business process", bpmnContent: BpmnSampleFilesHelper.ReadFileContentAsString());

            var businessProcessViewModels = await service.GetBusinessProcessList();

            var businessProcess = await repository.GetByName(businessProcessViewModels.First().Name);
            var instance = businessProcess.CreateInstance();
            await repository.Save(instance);

            await service.ExecuteBusinessProcess(id: instance.Id, null);
        }

        [Fact]
        public async Task execute_business_process_()
        {
            var urlPath = "http://localhost:8089/api/something/{id}";

            var serviceTask = new IAmAServiceTask("usertask", "name");

            serviceTask.SetExecutorDescription(new TaskExecutorDescription
            {
                Type = TaskExecutorType.RestApi,
                Url = urlPath
            });

            serviceTask.SetDataProvider(new InputDataProvider(source: DataProviderType.ExecutionContext));

            var aBusinessProcess = new BusinessProcess("id", "bp"
                , new IAmAnStartEvent("start", "start event")
                , new IAmAnEndEvent("end", "name"),
                new List<IAmAServiceTask>
                {
                        serviceTask
                }, null
                , null
                , null
                , new List<SequenceFlow>
                {
                        new()
                        {
                            SourceRef = "start",
                            TargetRef = "usertask"
                        },
                        new()
                        {
                            SourceRef = "usertask",
                            TargetRef = "end"
                        },
                }, null);

            var stubRestApiTaskExecutor = StubRestApiTaskExecutor.WhichIExpectedToBeCallWithUrl("http://localhost:8089/api/something/123", serviceTask);

            var businessProcessExecutorFactory = StubBusinessProcessElementExecutorAbstractFactory.WhichWhenCallCrateUserTaskExecutorIExpectToReturn(stubRestApiTaskExecutor);

            var repository = CreateRepository(await CreateDbContextAndMigrateDataBase());
            await repository.Save(aBusinessProcess, "content");

            var instance = aBusinessProcess.CreateInstance();

            await repository.Save(instance);

            IBusinessProcessExecutor businessProcessExecutor = new BusinessProcessExecutor(businessProcessExecutorFactory, repository);

            businessProcessExecutor.Execute(instance);

            businessProcessExecutor.Execute(instance);

            //stubRestApiTaskExecutor.Verify();

            //var businessProcess = await repository.GetBusinessProcessInstanceById(instance.Id);
            //businessProcess.Status.Should().Be(BusinessProcessStatus.CompletedSuccessfully);
        }

        [Fact]
        public async Task execute_business_process()
        {
            var urlPath = "http://localhost:8089/api/something";

            var userTask = new IAmAServiceTask("usertask", "name");

            userTask.SetExecutorDescription(new TaskExecutorDescription
            {
                Type = TaskExecutorType.RestApi,
                Url = urlPath
            });

            userTask.SetDataProvider(new InputDataProvider(source: DataProviderType.ExecutionContext));

            var aBusinessProcess = new BusinessProcess("id", "bp"
                , new IAmAnStartEvent("start", "start event")
                , new IAmAnEndEvent("end", "name"),
                new List<IAmAServiceTask>
                {
                        userTask,
                }, null
                , null
                , null
                , new List<SequenceFlow>
                {
                        new()
                        {
                            SourceRef = "start",
                            TargetRef = "usertask"
                        },
                        new()
                        {
                            SourceRef = "usertask",
                            TargetRef = "end"
                        },
                }, null);

            var stubRestApiTaskExecutor = StubRestApiTaskExecutor.WhichIExpectedToBeCallWithUrl(urlPath, userTask);

            var businessProcessExecutorFactory = StubBusinessProcessElementExecutorAbstractFactory.WhichWhenCallCrateUserTaskExecutorIExpectToReturn(stubRestApiTaskExecutor);

            var repository = CreateRepository(await CreateDbContextAndMigrateDataBase());
            await repository.Save(aBusinessProcess, "content");

            var instance = aBusinessProcess.CreateInstance();

            await repository.Save(instance);

            IBusinessProcessExecutor businessProcessExecutor = new BusinessProcessExecutor(businessProcessExecutorFactory, repository);

            businessProcessExecutor.Execute(instance);

            stubRestApiTaskExecutor.Verify();

            var businessProcess = await repository.GetBusinessProcessInstanceById(instance.Id);
            businessProcess.Status.Should().Be(BusinessProcessStatus.CompletedSuccessfully);
        }

        [Theory]
        [InlineData("https://github.com/")]
        [InlineData("https://google.com/")]
        public void execute_service_tasks(string url)
        {
            var serviceTask = new IAmAServiceTask("id", "name");

            serviceTask.SetExecutorDescription(new TaskExecutorDescription
            {
                Url = url,
                Type = TaskExecutorType.RestApi
            });

            IServiceTaskExecutor executor = new RestApiServiceTaskExecutor();
            executor.Execute(new BpmnExecutionContext(), serviceTask);
        }

        [Theory]
        [InlineData("url")]
        [InlineData("/api/com")]
        public void rest_api_task_executor_should_raise_exception_if_url_format_is_not_valid(string url)
        {
            var serviceTask = new IAmAServiceTask("id", "name");

            serviceTask.SetExecutorDescription(new TaskExecutorDescription
            {
                Url = url,
                Type = TaskExecutorType.RestApi
            });

            var executor = new RestApiServiceTaskExecutor();

            var action = () => executor.Execute(new BpmnExecutionContext(), serviceTask);
            action.Should().Throw<RengDomainException>();
        }

        [Fact]
        public void test2()
        {
            var json = BpmnSampleFilesHelper.LoadBpmFrom(this._orderPastaBpmnFilePath);

            var bpmn2 = JsonConvert.DeserializeObject<Bpmn2>(json);

            var businessProcess = BusinessProcessFactory.Create("OrderLunch", bpmn2);

            businessProcess.Name.Should().BeEquivalentTo("OrderLunch");

            businessProcess
                .Should().NotBeNull();

            businessProcess
                .GetStartEvent()
                .Should().NotBeNull();

            businessProcess
                .GetStartEvent()
                .GetOutgoings()
                .Count
                .Should().Be(1);

            businessProcess
                .GetStartEvent()
                .Name
                .Should().BeEquivalentTo("Start Event");

            businessProcess
                .GetEndEvent()
                .Should().NotBeNull();

            businessProcess
                .GetEndEvent()
                .Name
                .Should().BeEquivalentTo("End Event");

            businessProcess
                .GetUserTasks()
                .Count
                .Should().Be(1);

            businessProcess
                .GetUserTasks()
                .Should()
                .Contain(u => u.Name == "Receipie");
        }

        [Fact]
        public async Task compensate()
        {
            var serviceTask = new IAmAServiceTask("CalculatePayroll", "CalculatePayroll");

            serviceTask.SetHasCompensate(true);

            serviceTask.SetDataProvider(new InputDataProvider(DataProviderType.ExecutionContext));

            serviceTask.SetCompensate(new CompensateExecution
            {
                Type = TaskExecutorType.RestApi,
                Url = "http://localhost"
            });

            var executor = new RestApiServiceTaskExecutor();

            executor.Compensate(new BpmnExecutionContext(), serviceTask);
        }

        [Fact]
        public async Task name_should_be_unique()
        {
            IBpmnApplicationService service = CreateBpmnApplicationService();
            await service.CreateBusinessProcess("my-business-process", BpmnSampleFilesHelper.ReadFileContentAsString());

            var action = () => service.CreateBusinessProcess("my-business-process", BpmnSampleFilesHelper.ReadFileContentAsString()).Wait();
            action.Should().Throw<RengDomainException>();

            action.Should().Throw<RengDomainException>();
        }

        [Fact]
        public async Task execute_parallel_getaway()
        {
            var businessProcess = new BusinessProcess("id", "bp"
                , new IAmAnStartEvent("startEvent", "startEVent")
                , new IAmAnEndEvent("endEvent", "endEvent")
                , new List<IAmAServiceTask>
                                {
                                        new ("CalculatePayroll", "CalculatePayroll"),
                                        new ("CalculateTax", "CalculateTax"),
                                        new ("CalculateInsurance", "CalculateInsurance"),
                                        new ("CalculateLoan", "CalculateLoan"),
                                        new ("CalculatePayroll2", "CalculatePayroll2CalculatePayroll2"),
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

            var repository = CreateRepository(await CreateDbContextAndMigrateDataBase());
            await repository.Save(businessProcess, "content");
            var instance = businessProcess.CreateInstance();
            await repository.Save(instance);

            var factory = StubBusinessProcessElementExecutorAbstractFactory.WhichWhenCallCrateUserTaskExecutorIExpectToReturn(new NullServiceTaskExecutor());

            var executor = new BusinessProcessExecutor(factory, repository);
            executor.Execute(instance);

            var process = await repository.GetBusinessProcessInstanceById(instance.Id);

            process.Status.Should().Be(BusinessProcessStatus.CompletedSuccessfully);
        }

        [Fact]
        public async Task compensate_business_process()
        {
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

            var repository = CreateRepository(await CreateDbContextAndMigrateDataBase());
            await repository.Save(businessProcess, "content");
            var instance = businessProcess.CreateInstance();

            await repository.Save(instance);
            var stubServiceTaskExecutor = StubServiceTaskExecutor.New();
            var factory = StubBusinessProcessElementExecutorAbstractFactory.WhichWhenCallCrateUserTaskExecutorIExpectToReturn(stubServiceTaskExecutor);

            var executor = new BusinessProcessExecutor(factory, repository);
            executor.Execute(instance);

            ((StubServiceTaskExecutor)stubServiceTaskExecutor).VerifyThatNumberOfCalledIs(4);
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }
        private BpmnApplicationService CreateBpmnApplicationService()
        {
            return new BpmnApplicationService(CreateRepository(CreateDbContextAndMigrateDataBase().Result), new BusinessProcessElementExecutorAbstractFactory());
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
    }
}