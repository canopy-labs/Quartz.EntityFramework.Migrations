namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

internal class QuartzCronTrigger
{
    public string SchedName { get; set; } = null!;
    public string TriggerName { get; set; } = null!;
    public string TriggerGroup { get; set; } = null!;
    public string CronExpression { get; set; } = null!;
    public string? TimeZoneId { get; set; }

    public QuartzTrigger Trigger { get; set; } = null!;
}
