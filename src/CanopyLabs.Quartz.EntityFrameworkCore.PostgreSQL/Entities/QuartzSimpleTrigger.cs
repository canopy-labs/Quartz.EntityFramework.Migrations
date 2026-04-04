namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

internal class QuartzSimpleTrigger
{
    public string SchedName { get; set; } = null!;
    public string TriggerName { get; set; } = null!;
    public string TriggerGroup { get; set; } = null!;
    public long RepeatCount { get; set; }
    public long RepeatInterval { get; set; }
    public long TimesTriggered { get; set; }

    public QuartzTrigger Trigger { get; set; } = null!;
}
