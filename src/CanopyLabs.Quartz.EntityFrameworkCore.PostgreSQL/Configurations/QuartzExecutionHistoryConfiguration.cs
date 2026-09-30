using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Configurations;

/// <summary>
/// Quartz.NET 4.2's record of what a scheduler ran. Optional upstream: only a store configured
/// with <c>UseExecutionHistory()</c> touches it. No foreign key, on purpose: a history row
/// outlives the job and trigger it names.
/// </summary>
internal class QuartzExecutionHistoryConfiguration(string prefix, string? schema)
    : IEntityTypeConfiguration<QuartzExecutionHistory>
{
    public void Configure(EntityTypeBuilder<QuartzExecutionHistory> builder)
    {
        builder.ToTable($"{prefix}execution_history", schema);

        builder.HasKey(x => new { x.SchedName, x.EntryId });

        builder.Property(x => x.SchedName).HasColumnName("sched_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.EntryId).HasColumnName("entry_id").HasColumnType("text").IsRequired();
        builder.Property(x => x.InstanceName).HasColumnName("instance_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.JobName).HasColumnName("job_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.JobGroup).HasColumnName("job_group").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerName).HasColumnName("trigger_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerGroup).HasColumnName("trigger_group").HasColumnType("text").IsRequired();
        builder.Property(x => x.FiredTime).HasColumnName("fired_time").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.RunTime).HasColumnName("run_time").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.Succeeded).HasColumnName("succeeded").HasColumnType("bool").IsRequired();
        builder.Property(x => x.ErrorMessage).HasColumnName("error_message").HasColumnType("text");
        builder.Property(x => x.RetryAttempt).HasColumnName("retry_attempt").HasColumnType("integer").HasDefaultValue(0).IsRequired();
        builder.Property(x => x.RetryScheduled).HasColumnName("retry_scheduled").HasColumnType("bool").HasDefaultValue(false).IsRequired();
        builder.Property(x => x.ExecutionLog).HasColumnName("execution_log").HasColumnType("text");

        // (sched_name, fired_time) serves the age query and the retention sweep;
        // (sched_name, instance_name) serves the node filter.
        builder.HasIndex(x => new { x.SchedName, x.FiredTime }).HasDatabaseName($"idx_{prefix}eh_fired_time");
        builder.HasIndex(x => new { x.SchedName, x.InstanceName }).HasDatabaseName($"idx_{prefix}eh_inst");
    }
}
