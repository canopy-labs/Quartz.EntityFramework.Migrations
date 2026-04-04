using Microsoft.EntityFrameworkCore;
using Quartz.EntityFrameworkCore.Migrations.PostgreSQL.Configurations;

namespace Quartz.EntityFrameworkCore.Migrations.PostgreSQL;

public static class ModelBuilderExtensions
{
    public static ModelBuilder AddQuartzPostgreSql(
        this ModelBuilder modelBuilder,
        string prefix = "qrtz_",
        string? schema = null)
    {
        modelBuilder.ApplyConfiguration(new QuartzJobDetailConfiguration(prefix, schema));
        modelBuilder.ApplyConfiguration(new QuartzTriggerConfiguration(prefix, schema));
        modelBuilder.ApplyConfiguration(new QuartzCronTriggerConfiguration(prefix, schema));
        modelBuilder.ApplyConfiguration(new QuartzSimpleTriggerConfiguration(prefix, schema));
        modelBuilder.ApplyConfiguration(new QuartzSimpropTriggerConfiguration(prefix, schema));
        modelBuilder.ApplyConfiguration(new QuartzBlobTriggerConfiguration(prefix, schema));
        modelBuilder.ApplyConfiguration(new QuartzCalendarConfiguration(prefix, schema));
        modelBuilder.ApplyConfiguration(new QuartzPausedTriggerGroupConfiguration(prefix, schema));
        modelBuilder.ApplyConfiguration(new QuartzFiredTriggerConfiguration(prefix, schema));
        modelBuilder.ApplyConfiguration(new QuartzSchedulerStateConfiguration(prefix, schema));
        modelBuilder.ApplyConfiguration(new QuartzLockConfiguration(prefix, schema));

        return modelBuilder;
    }
}
