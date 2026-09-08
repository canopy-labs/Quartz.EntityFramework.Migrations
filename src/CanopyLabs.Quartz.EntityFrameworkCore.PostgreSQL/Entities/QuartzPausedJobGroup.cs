namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Entities;

internal class QuartzPausedJobGroup
{
    public string SchedName { get; set; } = null!;
    public string JobGroup { get; set; } = null!;
}
