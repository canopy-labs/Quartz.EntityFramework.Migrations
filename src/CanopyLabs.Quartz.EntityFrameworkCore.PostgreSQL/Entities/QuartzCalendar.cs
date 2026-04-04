namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

internal class QuartzCalendar
{
    public string SchedName { get; set; } = null!;
    public string CalendarName { get; set; } = null!;
    public byte[] Calendar { get; set; } = null!;
}
