namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

internal class QuartzPausedTriggerGroup
{
    public string SchedName { get; set; } = null!;
    public string TriggerGroup { get; set; } = null!;
}
