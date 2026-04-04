namespace Quartz.EntityFrameworkCore.Migrations.PostgreSQL.Entities;

internal class QuartzBlobTrigger
{
    public string SchedName { get; set; } = null!;
    public string TriggerName { get; set; } = null!;
    public string TriggerGroup { get; set; } = null!;
    public byte[]? BlobData { get; set; }

    public QuartzTrigger Trigger { get; set; } = null!;
}
