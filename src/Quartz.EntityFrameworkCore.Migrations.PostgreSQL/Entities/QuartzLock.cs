namespace Quartz.EntityFrameworkCore.Migrations.PostgreSQL.Entities;

internal class QuartzLock
{
    public string SchedName { get; set; } = null!;
    public string LockName { get; set; } = null!;
}
