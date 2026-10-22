using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Configurations;

/// <summary>
/// Quartz.NET 4.4's per-job rollup of the execution history: one row per job, written with each
/// history row so it outlives the history's retention. Optional upstream, like
/// <see cref="QuartzExecutionHistoryConfiguration"/>. No foreign key, on purpose: a 4.3 node
/// deleting a job must not trip on a row it has never heard of.
/// </summary>
internal class QuartzJobStatusConfiguration(string prefix, string? schema)
    : IEntityTypeConfiguration<QuartzJobStatus>
{
    public void Configure(EntityTypeBuilder<QuartzJobStatus> builder)
    {
        builder.ToTable($"{prefix}job_status", schema);

        builder.HasKey(x => new { x.SchedName, x.JobGroup, x.JobName });

        builder.Property(x => x.SchedName).HasColumnName("sched_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.JobGroup).HasColumnName("job_group").HasColumnType("text").IsRequired();
        builder.Property(x => x.JobName).HasColumnName("job_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.FirstFiredTime).HasColumnName("first_fired_time").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.LastFiredTime).HasColumnName("last_fired_time").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.LastResult).HasColumnName("last_result").HasColumnType("integer").IsRequired();
        builder.Property(x => x.LastRunTime).HasColumnName("last_run_time").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.LastInstanceName).HasColumnName("last_instance_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.LastEntryId).HasColumnName("last_entry_id").HasColumnType("text");
        builder.Property(x => x.LastSummary).HasColumnName("last_summary").HasColumnType("text");
        builder.Property(x => x.LastSuccessTime).HasColumnName("last_success_time").HasColumnType("bigint");
        builder.Property(x => x.LastFailureTime).HasColumnName("last_failure_time").HasColumnType("bigint");
        builder.Property(x => x.LastFailureMessage).HasColumnName("last_failure_message").HasColumnType("text");
        builder.Property(x => x.ConsecutiveFailures).HasColumnName("consecutive_failures").HasColumnType("integer").HasDefaultValue(0).IsRequired();
        builder.Property(x => x.RunCount).HasColumnName("run_count").HasColumnType("bigint").HasDefaultValue(0L).IsRequired();
        builder.Property(x => x.FailureCount).HasColumnName("failure_count").HasColumnType("bigint").HasDefaultValue(0L).IsRequired();
    }
}
