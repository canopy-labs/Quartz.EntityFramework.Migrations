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
        // 4.4: what the run reported through IJobExecutionContext.Result, and (only with
        // ExecutionHistoryOptions.RecordInput) what it was given. All nullable, no default.
        builder.Property(x => x.Result).HasColumnName("result").HasColumnType("integer");
        builder.Property(x => x.Summary).HasColumnName("summary").HasColumnType("text");
        builder.Property(x => x.Metrics).HasColumnName("metrics").HasColumnType("text");
        builder.Property(x => x.Manual).HasColumnName("manual").HasColumnType("bool");
        builder.Property(x => x.FireInstanceId).HasColumnName("fire_instance_id").HasColumnType("text");
        builder.Property(x => x.JobInput).HasColumnName("job_input").HasColumnType("text");
        builder.Property(x => x.JobInputTooLarge).HasColumnName("job_input_too_large").HasColumnType("bool");

        // (sched_name, fired_time) serves the age query and the retention sweep;
        // (sched_name, instance_name) serves the node filter.
        builder.HasIndex(x => new { x.SchedName, x.FiredTime }).HasDatabaseName($"idx_{prefix}eh_fired_time");
        builder.HasIndex(x => new { x.SchedName, x.InstanceName }).HasDatabaseName($"idx_{prefix}eh_inst");
        // 4.4: one job's history by time. 4.6: one trigger's history by time, and a firing's
        // row by its fire instance id.
        builder.HasIndex(x => new { x.SchedName, x.JobGroup, x.JobName, x.FiredTime }).HasDatabaseName($"idx_{prefix}eh_job_time");
        builder.HasIndex(x => new { x.SchedName, x.FireInstanceId }).HasDatabaseName($"idx_{prefix}eh_fire_inst");
        builder.HasIndex(x => new { x.SchedName, x.TriggerGroup, x.TriggerName, x.FiredTime }).HasDatabaseName($"idx_{prefix}eh_trig_time");
    }
}
