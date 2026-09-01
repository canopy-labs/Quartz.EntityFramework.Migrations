using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Configurations;

internal class QuartzFiredTriggerConfiguration(string prefix, string? schema)
    : IEntityTypeConfiguration<QuartzFiredTrigger>
{
    public void Configure(EntityTypeBuilder<QuartzFiredTrigger> builder)
    {
        builder.ToTable($"{prefix}fired_triggers", schema);

        builder.HasKey(x => new { x.SchedName, x.EntryId });

        builder.Property(x => x.SchedName).HasColumnName("sched_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.EntryId).HasColumnName("entry_id").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerName).HasColumnName("trigger_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerGroup).HasColumnName("trigger_group").HasColumnType("text").IsRequired();
        builder.Property(x => x.InstanceName).HasColumnName("instance_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.FiredTime).HasColumnName("fired_time").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.SchedTime).HasColumnName("sched_time").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.Priority).HasColumnName("priority").HasColumnType("integer").IsRequired();
        builder.Property(x => x.State).HasColumnName("state").HasColumnType("text").IsRequired();
        builder.Property(x => x.JobName).HasColumnName("job_name").HasColumnType("text");
        builder.Property(x => x.JobGroup).HasColumnName("job_group").HasColumnType("text");
        builder.Property(x => x.IsNonconcurrent).HasColumnName("is_nonconcurrent").HasColumnType("bool").IsRequired();
        builder.Property(x => x.RequestsRecovery).HasColumnName("requests_recovery").HasColumnType("bool");
        builder.Property(x => x.ExecutionGroup).HasColumnName("execution_group").HasColumnType("varchar(200)");

        builder.HasIndex(x => new { x.SchedName, x.InstanceName, x.RequestsRecovery }).HasDatabaseName($"idx_{prefix}ft_inst_job_req_rcvry");
        builder.HasIndex(x => new { x.SchedName, x.JobName, x.JobGroup }).HasDatabaseName($"idx_{prefix}ft_j_g");
        builder.HasIndex(x => new { x.SchedName, x.TriggerName, x.TriggerGroup }).HasDatabaseName($"idx_{prefix}ft_t_g");
    }
}
