using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Configurations;

internal class QuartzCronTriggerConfiguration(string prefix, string? schema)
    : IEntityTypeConfiguration<QuartzCronTrigger>
{
    public void Configure(EntityTypeBuilder<QuartzCronTrigger> builder)
    {
        builder.ToTable($"{prefix}cron_triggers", schema);

        builder.HasKey(x => new { x.SchedName, x.TriggerName, x.TriggerGroup });

        builder.Property(x => x.SchedName).HasColumnName("sched_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerName).HasColumnName("trigger_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerGroup).HasColumnName("trigger_group").HasColumnType("text").IsRequired();
        builder.Property(x => x.CronExpression).HasColumnName("cron_expression").HasColumnType("text").IsRequired();
        builder.Property(x => x.TimeZoneId).HasColumnName("time_zone_id").HasColumnType("text");

        builder.HasOne(x => x.Trigger)
            .WithOne()
            .HasForeignKey<QuartzCronTrigger>(x => new { x.SchedName, x.TriggerName, x.TriggerGroup })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
