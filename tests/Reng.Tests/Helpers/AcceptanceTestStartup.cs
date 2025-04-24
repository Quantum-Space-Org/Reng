using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reng.BPMN.ApplicationService;
using Reng.BPMN.Domain;
using System.Data.Common;
using Reng.BPMN.Domain.Domain;

namespace Reng.Tests.Helpers
{
    public class AcceptanceTestStartup
    {
        public IConfiguration Configuration { get; }

        public AcceptanceTestStartup(IConfiguration configuration)
            => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddScoped<IBpmnApplicationService, BpmnApplicationService>();
            services.AddScoped<IBusinessProcessRepository, BusinessProcessRepository>();
            services.AddSingleton<BusinessProcessDbContext>();
            services.AddSingleton<IBusinessProcessElementExecutorAbstractFactory, BusinessProcessElementExecutorAbstractFactory>();
            services.AddSingleton(sp => CreateDbContextAndMigrateDataBase());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var service = app.ApplicationServices.GetService(typeof(BusinessProcessDbContext));

            ((BusinessProcessDbContext)service).Database.EnsureCreatedAsync().Wait();
        }

        private DbContextOptions<BusinessProcessDbContext> CreateDbContextAndMigrateDataBase()
        {

            var options = new DbContextOptionsBuilder<BusinessProcessDbContext>().UseSqlite(CreateInMemoryDatabase()).Options;

            return options;

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
    }
}
