using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Reng.BPMN.Domain;
using Reng.BPMN.Domain.Domain;

namespace Reng.BPMN.ApplicationService;

public class BusinessProcessDbContext : DbContext
{
    public DbSet<BusinessProcess> BusinessProcesses { get; set; }
    public DbSet<BusinessProcessInstance> BusinessProcesseInstances { get; set; }
    public DbSet<BusinessProcessContent> BusinessProcessesContent { get; set; }

    public BusinessProcessDbContext(DbContextOptions<BusinessProcessDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BusinessProcess>()
            .HasKey(bp => bp.Id);

        modelBuilder.Entity<BusinessProcessContent>()
            .HasKey(bp => bp.BusinessProcessId);

        modelBuilder.Entity<BusinessProcessContent>()
            .Property(bp => bp.Content)
            .HasMaxLength(int.MaxValue);

        modelBuilder.Entity<BusinessProcessContent>()
            .HasOne(bp => bp.BusinessProcess);

        modelBuilder.Entity<BusinessProcessLog>()
            .Property(bp => bp.ElementName)
            .IsRequired(false);

        modelBuilder.Entity<BusinessProcessLog>()
            .Property(bp => bp.Context)
            .HasConversion(
                context => JsonConvert.SerializeObject(context),
                s => JsonConvert.DeserializeObject<BpmnExecutionContext>(s));


        modelBuilder.Entity<InputDataProvider>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<IAmAServiceTask>()
          .Property(bp => bp.TaskExecutorDescription)
          .HasConversion(
              context => JsonConvert.SerializeObject(context, new JsonSerializerSettings
              {
                  ReferenceLoopHandling = ReferenceLoopHandling.Ignore
              }),
              s => JsonConvert.DeserializeObject<TaskExecutorDescription>(s,
              new JsonSerializerSettings
              {
                  ReferenceLoopHandling = ReferenceLoopHandling.Ignore
              }));

        modelBuilder.Entity<IAmAServiceTask>()
          .Property(bp => bp.CompensateDescription)
          .HasConversion(
              context => JsonConvert.SerializeObject(context, new JsonSerializerSettings
              {
                  ReferenceLoopHandling = ReferenceLoopHandling.Ignore
              }),
              s => JsonConvert.DeserializeObject<CompensateExecution>(s,
              new JsonSerializerSettings
              {
                  ReferenceLoopHandling = ReferenceLoopHandling.Ignore
              }));

        modelBuilder.Entity<BusinessProcessInstance>()
         .Property(bp => bp.CompensateStack)
         .HasConversion(
             context => JsonConvert.SerializeObject(context, new JsonSerializerSettings
             {
                 ReferenceLoopHandling = ReferenceLoopHandling.Ignore
             }),
             s => JsonConvert.DeserializeObject<Stack<IAmABusinessProcessElement>>(s,
             new JsonSerializerSettings
             {
                 ReferenceLoopHandling = ReferenceLoopHandling.Ignore
             }));

        modelBuilder.Entity<IAmABusinessProcessElement>()
          .Property(bp => bp.Name)
          .IsRequired(false);

        modelBuilder.Entity<IAmABusinessProcessElement>()
      .Property(bp => bp.Token)
      .IsRequired(false);

        base.OnModelCreating(modelBuilder);
    }
}