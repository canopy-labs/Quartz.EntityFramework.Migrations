using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Configurations;

internal class QuartzPausedTriggerGroupConfiguration(string prefix, string? schema)
    : IEntityTypeConfiguration<QuartzPausedTriggerGroup>
{
    public void Configure(EntityTypeBuilder<QuartzPausedTriggerGroup> builder)
    {
        builder.ToTable($"{prefix}paused_trigger_grps", schema);

        builder.HasKey(x => new { x.SchedName, x.TriggerGroup });

        builder.Property(x => x.SchedName).HasColumnName("sched_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerGroup).HasColumnName("trigger_group").HasColumnType("text").IsRequired();
    }
}
