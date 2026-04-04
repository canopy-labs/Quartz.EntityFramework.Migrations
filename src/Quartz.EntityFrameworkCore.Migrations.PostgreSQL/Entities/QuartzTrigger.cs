namespace Quartz.EntityFrameworkCore.Migrations.PostgreSQL.Entities;

internal class QuartzTrigger
{
    public string SchedName { get; set; } = null!;
    public string TriggerName { get; set; } = null!;
    public string TriggerGroup { get; set; } = null!;
    public string JobName { get; set; } = null!;
    public string JobGroup { get; set; } = null!;
    public string? Description { get; set; }
    public long? NextFireTime { get; set; }
    public long? PrevFireTime { get; set; }
    public int? Priority { get; set; }
    public string TriggerState { get; set; } = null!;
    public string TriggerType { get; set; } = null!;
    public long StartTime { get; set; }
    public long? EndTime { get; set; }
    public string? CalendarName { get; set; }
    public short? MisfireInstr { get; set; }
    public long? MisfireOrigFireTime { get; set; }
    public byte[]? JobData { get; set; }

    public QuartzJobDetail JobDetail { get; set; } = null!;
}
