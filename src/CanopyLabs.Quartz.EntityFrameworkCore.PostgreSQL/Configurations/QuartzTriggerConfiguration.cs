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
        builder.Property(x => x.RetryPolicy).HasColumnName("retry_policy").HasColumnType("varchar(250)");
        builder.Property(x => x.RetryAttempt).HasColumnName("retry_attempt").HasColumnType("integer");
        // 4.2 continuations: the trigger this one waits on (a trigger key, so declared like
        // trigger_name/trigger_group but nullable) and the ContinuationCondition flags that
        // release it. Upstream adds no index for them; the settlement lookup rides nft_st.
        builder.Property(x => x.ContinuesTriggerName).HasColumnName("continues_trigger_name").HasColumnType("text");
        builder.Property(x => x.ContinuesTriggerGroup).HasColumnName("continues_trigger_group").HasColumnType("text");
        builder.Property(x => x.ContinuationCondition).HasColumnName("continuation_condition").HasColumnType("integer");
        // 4.3: what to do when a firing lands while the previous one still runs, and who paused
        // the trigger, when and why. All nullable, no default; no index added.
        builder.Property(x => x.OverlapPolicy).HasColumnName("overlap_policy").HasColumnType("integer");
        builder.Property(x => x.PauseReason).HasColumnName("pause_reason").HasColumnType("varchar(250)");
        builder.Property(x => x.PausedBy).HasColumnName("paused_by").HasColumnType("varchar(200)");
        builder.Property(x => x.PausedAt).HasColumnName("paused_at").HasColumnType("bigint");
        builder.Property(x => x.JobData).HasColumnName("job_data").HasColumnType("bytea");

        builder.HasOne(x => x.JobDetail)
            .WithMany()
            .HasForeignKey(x => new { x.SchedName, x.JobName, x.JobGroup });

        // Every AdoJobStore statement filters sched_name first, so every index leads with it.
        // 4.0 widened idx_qrtz_t_nft_st to cover the acquire query end to end: two equalities
        // (sched_name, trigger_state), the range on next_fire_time, then priority DESC and
        // misfire_instr, which the query orders and filters by. That makes the standalone
        // idx_qrtz_t_next_fire_time redundant (nft_st is a covering superset), so upstream
        // dropped it along with idx_qrtz_j_req_recovery on qrtz_job_details.
        builder.HasIndex(x => new { x.SchedName, x.JobName, x.JobGroup }).HasDatabaseName($"idx_{prefix}t_j");
        builder.HasIndex(x => new { x.SchedName, x.CalendarName }).HasDatabaseName($"idx_{prefix}t_c");
        builder.HasIndex(x => new { x.SchedName, x.TriggerGroup, x.TriggerName }).HasDatabaseName($"idx_{prefix}t_g_n");
        builder.HasIndex(x => new { x.SchedName, x.TriggerState, x.NextFireTime, x.Priority, x.MisfireInstr })
            .IsDescending(false, false, false, true, false)
            .HasDatabaseName($"idx_{prefix}t_nft_st");
    }
}
