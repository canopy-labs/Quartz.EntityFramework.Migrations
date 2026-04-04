using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quartz.EntityFrameworkCore.Migrations.PostgreSQL.Entities;

namespace Quartz.EntityFrameworkCore.Migrations.PostgreSQL.Configurations;

internal class QuartzSimpropTriggerConfiguration(string prefix, string? schema)
    : IEntityTypeConfiguration<QuartzSimpropTrigger>
{
    public void Configure(EntityTypeBuilder<QuartzSimpropTrigger> builder)
    {
        builder.ToTable($"{prefix}simprop_triggers", schema);

        builder.HasKey(x => new { x.SchedName, x.TriggerName, x.TriggerGroup });

        builder.Property(x => x.SchedName).HasColumnName("sched_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerName).HasColumnName("trigger_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.TriggerGroup).HasColumnName("trigger_group").HasColumnType("text").IsRequired();
        builder.Property(x => x.StrProp1).HasColumnName("str_prop_1").HasColumnType("text");
        builder.Property(x => x.StrProp2).HasColumnName("str_prop_2").HasColumnType("text");
        builder.Property(x => x.StrProp3).HasColumnName("str_prop_3").HasColumnType("text");
        builder.Property(x => x.IntProp1).HasColumnName("int_prop_1").HasColumnType("integer");
        builder.Property(x => x.IntProp2).HasColumnName("int_prop_2").HasColumnType("integer");
        builder.Property(x => x.LongProp1).HasColumnName("long_prop_1").HasColumnType("bigint");
        builder.Property(x => x.LongProp2).HasColumnName("long_prop_2").HasColumnType("bigint");
        builder.Property(x => x.DecProp1).HasColumnName("dec_prop_1").HasColumnType("numeric");
        builder.Property(x => x.DecProp2).HasColumnName("dec_prop_2").HasColumnType("numeric");
        builder.Property(x => x.BoolProp1).HasColumnName("bool_prop_1").HasColumnType("bool");
        builder.Property(x => x.BoolProp2).HasColumnName("bool_prop_2").HasColumnType("bool");
        builder.Property(x => x.TimeZoneId).HasColumnName("time_zone_id").HasColumnType("text");

        builder.HasOne(x => x.Trigger)
            .WithOne()
            .HasForeignKey<QuartzSimpropTrigger>(x => new { x.SchedName, x.TriggerName, x.TriggerGroup })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
