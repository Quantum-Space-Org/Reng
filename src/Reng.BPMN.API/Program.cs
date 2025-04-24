using Microsoft.EntityFrameworkCore;
using Reng.BPMN.ApplicationService;
using System.Data.Common;
using Elastic.Apm.SerilogEnricher;
using Elastic.CommonSchema.Serilog;
using Microsoft.Data.Sqlite;
using Reng.BPMN.Domain.Domain;
using Reng.BPMN.API;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .Enrich.WithElasticApmCorrelationInfo()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(builder.Configuration.GetValue<string>("elasticSearch:url")))
    {
        CustomFormatter = new EcsTextFormatter()
    })
    .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();


builder.Logging.AddSerilog(logger);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

builder.Services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddScoped<IBpmnApplicationService, BpmnApplicationService>();


builder.Services.AddTransient<IBpmnApplicationService, BpmnApplicationService>();
builder.Services.AddScoped<IBusinessProcessRepository, BusinessProcessRepository>();
builder.Services.AddSingleton<BusinessProcessDbContext>();
builder.Services.AddSingleton<IBusinessProcessElementExecutorAbstractFactory, BusinessProcessElementExecutorAbstractFactory>();
builder.Services.AddSingleton<DbContextOptions<BusinessProcessDbContext>>(sp => CreateDbContextAndMigrateDataBase());


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseCors(c =>
    c.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()
);
app.UseAuthorization();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

var dbContext = app.Services.GetService(typeof(BusinessProcessDbContext));

((BusinessProcessDbContext)dbContext).Database.EnsureCreatedAsync().Wait();

app.Run();



DbContextOptions<BusinessProcessDbContext> CreateDbContextAndMigrateDataBase()
{

    var options = new DbContextOptionsBuilder<BusinessProcessDbContext>().UseSqlite(CreateInMemoryDatabase()).Options;

    return options;

}

BusinessProcessRepository CreateRepository(BusinessProcessDbContext context)
{
    var businessProcessRepository = new BusinessProcessRepository(context);
    return businessProcessRepository;
}

static DbConnection CreateInMemoryDatabase()
{
    var connection = new SqliteConnection("Filename=:memory:");

    connection.Open();

    return connection;
}