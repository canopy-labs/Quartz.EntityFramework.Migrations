using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Configurations;

internal class QuartzPausedJobGroupConfiguration(string prefix, string? schema)
    : IEntityTypeConfiguration<QuartzPausedJobGroup>
{
    public void Configure(EntityTypeBuilder<QuartzPausedJobGroup> builder)
    {
        builder.ToTable($"{prefix}paused_job_grps", schema);

        builder.HasKey(x => new { x.SchedName, x.JobGroup });

        builder.Property(x => x.SchedName).HasColumnName("sched_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.JobGroup).HasColumnName("job_group").HasColumnType("text").IsRequired();
        builder.Property(x => x.PauseReason).HasColumnName("pause_reason").HasColumnType("varchar(250)");
        builder.Property(x => x.PausedBy).HasColumnName("paused_by").HasColumnType("varchar(200)");
        builder.Property(x => x.PausedAt).HasColumnName("paused_at").HasColumnType("bigint");
    }
}
