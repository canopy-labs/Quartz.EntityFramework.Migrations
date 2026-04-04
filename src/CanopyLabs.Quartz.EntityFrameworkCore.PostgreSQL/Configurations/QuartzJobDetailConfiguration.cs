using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Configurations;

internal class QuartzJobDetailConfiguration(string prefix, string? schema)
    : IEntityTypeConfiguration<QuartzJobDetail>
{
    public void Configure(EntityTypeBuilder<QuartzJobDetail> builder)
    {
        builder.ToTable($"{prefix}job_details", schema);

        builder.HasKey(x => new { x.SchedName, x.JobName, x.JobGroup });

        builder.Property(x => x.SchedName).HasColumnName("sched_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.JobName).HasColumnName("job_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.JobGroup).HasColumnName("job_group").HasColumnType("text").IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasColumnType("text");
        builder.Property(x => x.JobClassName).HasColumnName("job_class_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.IsDurable).HasColumnName("is_durable").HasColumnType("bool").IsRequired();
        builder.Property(x => x.IsNonconcurrent).HasColumnName("is_nonconcurrent").HasColumnType("bool").IsRequired();
        builder.Property(x => x.IsUpdateData).HasColumnName("is_update_data").HasColumnType("bool").IsRequired();
        builder.Property(x => x.RequestsRecovery).HasColumnName("requests_recovery").HasColumnType("bool").IsRequired();
        builder.Property(x => x.JobData).HasColumnName("job_data").HasColumnType("bytea");

        builder.HasIndex(x => x.RequestsRecovery).HasDatabaseName($"idx_{prefix}j_req_recovery");
    }
}
