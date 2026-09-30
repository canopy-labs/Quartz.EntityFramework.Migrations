namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

internal class QuartzPausedTriggerGroup
{
    public string SchedName { get; set; } = null!;
    public string TriggerGroup { get; set; } = null!;
    public string? PauseReason { get; set; }
    public string? PausedBy { get; set; }
    public long? PausedAt { get; set; }
}
