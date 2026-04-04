using Microsoft.EntityFrameworkCore;
using CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL;

namespace CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL.Tests;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddQuartzPostgreSql();
    }
}
