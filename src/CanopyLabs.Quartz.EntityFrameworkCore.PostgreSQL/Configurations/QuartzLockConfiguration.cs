using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Configurations;

internal class QuartzLockConfiguration(string prefix, string? schema)
    : IEntityTypeConfiguration<QuartzLock>
{
    public void Configure(EntityTypeBuilder<QuartzLock> builder)
    {
        builder.ToTable($"{prefix}locks", schema);

        builder.HasKey(x => new { x.SchedName, x.LockName });

        builder.Property(x => x.SchedName).HasColumnName("sched_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.LockName).HasColumnName("lock_name").HasColumnType("text").IsRequired();
    }
}
