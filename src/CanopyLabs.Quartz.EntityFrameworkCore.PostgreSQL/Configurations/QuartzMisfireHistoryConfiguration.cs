using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Configurations;

/// <summary>
/// Quartz.NET 4.2's record of what a scheduler missed. Optional upstream, like
/// <see cref="QuartzExecutionHistoryConfiguration"/>, and likewise has no foreign key.
/// </summary>
internal class QuartzMisfireHistoryConfiguration(string prefix, string? schema)
    : IEntityTypeConfiguration<QuartzMisfireHistory>
{
    public void Configure(EntityTypeBuilder<QuartzMisfireHistory> builder)
    {
        builder.ToTable($"{prefix}misfire_history", schema);

        builder.HasKey(x => new { x.SchedName, x.EntryId });

        builder.Property(x => x.SchedName).HasColumnName("sched_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.EntryId).HasColumnName("entry_id").HasColumnType("text").IsRequired();
        builder.Property(x => x.InstanceName).HasColumnName("instance_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerName).HasColumnName("trigger_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerGroup).HasColumnName("trigger_group").HasColumnType("text").IsRequired();
        builder.Property(x => x.JobName).HasColumnName("job_name").HasColumnType("text");
        builder.Property(x => x.JobGroup).HasColumnName("job_group").HasColumnType("text");
        builder.Property(x => x.MisfireTime).HasColumnName("misfire_time").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.SchedTime).HasColumnName("sched_time").HasColumnType("bigint");

        builder.HasIndex(x => new { x.SchedName, x.MisfireTime }).HasDatabaseName($"idx_{prefix}mh_misfire_time");
        builder.HasIndex(x => new { x.SchedName, x.InstanceName }).HasDatabaseName($"idx_{prefix}mh_inst");
    }
}
