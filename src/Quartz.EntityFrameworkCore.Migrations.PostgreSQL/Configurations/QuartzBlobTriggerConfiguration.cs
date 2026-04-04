using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quartz.EntityFrameworkCore.Migrations.PostgreSQL.Entities;

namespace Quartz.EntityFrameworkCore.Migrations.PostgreSQL.Configurations;

internal class QuartzBlobTriggerConfiguration(string prefix, string? schema)
    : IEntityTypeConfiguration<QuartzBlobTrigger>
{
    public void Configure(EntityTypeBuilder<QuartzBlobTrigger> builder)
    {
        builder.ToTable($"{prefix}blob_triggers", schema);

        builder.HasKey(x => new { x.SchedName, x.TriggerName, x.TriggerGroup });

        builder.Property(x => x.SchedName).HasColumnName("sched_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerName).HasColumnName("trigger_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerGroup).HasColumnName("trigger_group").HasColumnType("text").IsRequired();
        builder.Property(x => x.BlobData).HasColumnName("blob_data").HasColumnType("bytea");

        builder.HasOne(x => x.Trigger)
            .WithOne()
            .HasForeignKey<QuartzBlobTrigger>(x => new { x.SchedName, x.TriggerName, x.TriggerGroup })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
