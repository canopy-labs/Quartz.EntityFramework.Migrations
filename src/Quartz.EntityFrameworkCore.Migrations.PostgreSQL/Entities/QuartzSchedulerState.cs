namespace Quartz.EntityFrameworkCore.Migrations.PostgreSQL.Entities;

internal class QuartzSchedulerState
{
    public string SchedName { get; set; } = null!;
    public string InstanceName { get; set; } = null!;
    public long LastCheckinTime { get; set; }
    public long CheckinInterval { get; set; }
}
