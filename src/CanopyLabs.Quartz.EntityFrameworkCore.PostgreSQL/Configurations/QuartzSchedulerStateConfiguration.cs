using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Configurations;

internal class QuartzSchedulerStateConfiguration(string prefix, string? schema)
    : IEntityTypeConfiguration<QuartzSchedulerState>
{
    public void Configure(EntityTypeBuilder<QuartzSchedulerState> builder)
    {
        builder.ToTable($"{prefix}scheduler_state", schema);

        builder.HasKey(x => new { x.SchedName, x.InstanceName });

        builder.Property(x => x.SchedName).HasColumnName("sched_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.InstanceName).HasColumnName("instance_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.LastCheckinTime).HasColumnName("last_checkin_time").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.CheckinInterval).HasColumnName("checkin_interval").HasColumnType("bigint").IsRequired();
        // 4.6: the Quartz version the node runs ('4.6.0'), written at every check-in. NULL is a
        // node older than 4.6; a 4.6 node keeps taking TRIGGER_ACCESS while any live row reads it.
        builder.Property(x => x.QuartzVersion).HasColumnName("quartz_version").HasColumnType("varchar(20)");
    }
}
