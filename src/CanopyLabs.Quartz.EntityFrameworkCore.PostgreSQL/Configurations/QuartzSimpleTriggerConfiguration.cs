using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Configurations;

internal class QuartzSimpleTriggerConfiguration(string prefix, string? schema)
    : IEntityTypeConfiguration<QuartzSimpleTrigger>
{
    public void Configure(EntityTypeBuilder<QuartzSimpleTrigger> builder)
    {
        builder.ToTable($"{prefix}simple_triggers", schema);

        builder.HasKey(x => new { x.SchedName, x.TriggerName, x.TriggerGroup });

        builder.Property(x => x.SchedName).HasColumnName("sched_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerName).HasColumnName("trigger_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerGroup).HasColumnName("trigger_group").HasColumnType("text").IsRequired();
        builder.Property(x => x.RepeatCount).HasColumnName("repeat_count").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.RepeatInterval).HasColumnName("repeat_interval").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.TimesTriggered).HasColumnName("times_triggered").HasColumnType("bigint").IsRequired();

        builder.HasOne(x => x.Trigger)
            .WithOne()
            .HasForeignKey<QuartzSimpleTrigger>(x => new { x.SchedName, x.TriggerName, x.TriggerGroup })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
