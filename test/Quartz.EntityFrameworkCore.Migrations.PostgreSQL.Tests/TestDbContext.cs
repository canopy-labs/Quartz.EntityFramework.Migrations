using Microsoft.EntityFrameworkCore;
using Quartz.EntityFrameworkCore.Migrations.PostgreSQL;

namespace Quartz.EntityFrameworkCore.Migrations.PostgreSQL.Tests;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddQuartzPostgreSql();
    }
}
