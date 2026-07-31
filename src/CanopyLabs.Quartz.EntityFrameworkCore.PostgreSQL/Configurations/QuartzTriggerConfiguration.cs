using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Configurations;

internal class QuartzTriggerConfiguration(string prefix, string? schema)
    : IEntityTypeConfiguration<QuartzTrigger>
{
    public void Configure(EntityTypeBuilder<QuartzTrigger> builder)
    {
        builder.ToTable($"{prefix}triggers", schema);

        builder.HasKey(x => new { x.SchedName, x.TriggerName, x.TriggerGroup });

        builder.Property(x => x.SchedName).HasColumnName("sched_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerName).HasColumnName("trigger_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerGroup).HasColumnName("trigger_group").HasColumnType("text").IsRequired();
        builder.Property(x => x.JobName).HasColumnName("job_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.JobGroup).HasColumnName("job_group").HasColumnType("text").IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasColumnType("text");
        builder.Property(x => x.NextFireTime).HasColumnName("next_fire_time").HasColumnType("bigint");
        builder.Property(x => x.PrevFireTime).HasColumnName("prev_fire_time").HasColumnType("bigint");
        builder.Property(x => x.Priority).HasColumnName("priority").HasColumnType("integer");
        builder.Property(x => x.TriggerState).HasColumnName("trigger_state").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerType).HasColumnName("trigger_type").HasColumnType("text").IsRequired();
        builder.Property(x => x.StartTime).HasColumnName("start_time").HasColumnType("bigint").IsRequired();
        builder.Property(x => x.EndTime).HasColumnName("end_time").HasColumnType("bigint");
        builder.Property(x => x.CalendarName).HasColumnName("calendar_name").HasColumnType("text");
        builder.Property(x => x.MisfireInstr).HasColumnName("misfire_instr").HasColumnType("smallint");
        builder.Property(x => x.MisfireOrigFireTime).HasColumnName("misfire_orig_fire_time").HasColumnType("bigint");
        builder.Property(x => x.ExecutionGroup).HasColumnName("execution_group").HasColumnType("varchar(200)");
        builder.Property(x => x.PreferredNode).HasColumnName("preferred_node").HasColumnType("varchar(200)");
        builder.Property(x => x.PreferredNodeAuto).HasColumnName("preferred_node_auto").HasColumnType("bool").HasDefaultValue(false).IsRequired();
        builder.Property(x => x.JobData).HasColumnName("job_data").HasColumnType("bytea");

        builder.HasOne(x => x.JobDetail)
            .WithMany()
            .HasForeignKey(x => new { x.SchedName, x.JobName, x.JobGroup });

        builder.HasIndex(x => x.NextFireTime).HasDatabaseName($"idx_{prefix}t_next_fire_time");
        builder.HasIndex(x => x.TriggerState).HasDatabaseName($"idx_{prefix}t_state");
        builder.HasIndex(x => new { x.NextFireTime, x.TriggerState }).HasDatabaseName($"idx_{prefix}t_nft_st");
    }
}
