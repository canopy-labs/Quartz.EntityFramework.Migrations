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
    }
}
