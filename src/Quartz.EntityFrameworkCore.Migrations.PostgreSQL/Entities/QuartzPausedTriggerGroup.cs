namespace Quartz.EntityFrameworkCore.Migrations.PostgreSQL.Entities;

internal class QuartzPausedTriggerGroup
{
    public string SchedName { get; set; } = null!;
    public string TriggerGroup { get; set; } = null!;
}
