using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quartz.EntityFrameworkCore.Migrations.PostgreSQL.Entities;

namespace Quartz.EntityFrameworkCore.Migrations.PostgreSQL.Configurations;

internal class QuartzCalendarConfiguration(string prefix, string? schema)
    : IEntityTypeConfiguration<QuartzCalendar>
{
    public void Configure(EntityTypeBuilder<QuartzCalendar> builder)
    {
        builder.ToTable($"{prefix}calendars", schema);

        builder.HasKey(x => new { x.SchedName, x.CalendarName });

        builder.Property(x => x.SchedName).HasColumnName("sched_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.CalendarName).HasColumnName("calendar_name").HasColumnType("text").IsRequired();
        builder.Property(x => x.Calendar).HasColumnName("calendar").HasColumnType("bytea").IsRequired();
    }
}
