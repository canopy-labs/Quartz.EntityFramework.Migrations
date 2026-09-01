# CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL

[![CI](https://github.com/canopy-labs/Quartz.EntityFramework.Migrations/actions/workflows/ci.yml/badge.svg)](https://github.com/canopy-labs/Quartz.EntityFramework.Migrations/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL)](https://www.nuget.org/packages/CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL)

EF Core migration support for [Quartz.NET](https://www.quartz-scheduler.net/) PostgreSQL tables.

Instead of running raw SQL scripts to create Quartz.NET's database tables, this library lets them participate in your EF Core migrations.

## Installation

```bash
dotnet add package CanopyLabs.Quartz.EntityFrameworkCore.PostgreSQL
```

## Usage

In your `DbContext.OnModelCreating`:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Add Quartz.NET tables with default settings (qrtz_ prefix, public schema)
    modelBuilder.AddQuartzPostgreSql();
}
```

Then generate a migration:

```bash
dotnet ef migrations add AddQuartzTables
dotnet ef database update
```

### Custom Schema

```csharp
modelBuilder.AddQuartzPostgreSql(schema: "quartz");
```

When using a custom schema, configure Quartz.NET's table prefix to match:

```csharp
var properties = new NameValueCollection
{
    ["quartz.jobStore.tablePrefix"] = "quartz.qrtz_"
};
```

### Custom Prefix

```csharp
modelBuilder.AddQuartzPostgreSql(prefix: "myapp_qrtz_", schema: "quartz");
```

## Version Compatibility

| Package Version | Quartz.NET Version | .NET |
|---|---|---|
| 3.20.x | 3.18.x, 3.19.x, 3.20.x | 8, 9, 10 |
| 3.19.x | 3.18.x, 3.19.x | 8, 9, 10 |
| 3.17.x | 3.17.x | 8, 9, 10 |

Match the major.minor version of this package to your Quartz.NET version.

A newer package works with an older Quartz.NET; the reverse does not. Every column
added so far is nullable or defaulted, and Quartz.NET 3.18+ probes for the columns it
needs rather than requiring them, so a column a given Quartz.NET version does not know
about is simply unused. 3.19.x added three columns on top of 3.17.x:

| Column | Table | Added in |
|---|---|---|
| `execution_group` | `qrtz_triggers`, `qrtz_fired_triggers` | Quartz.NET 3.18.0 |
| `preferred_node` | `qrtz_triggers` | Quartz.NET 3.19.0 |
| `preferred_node_auto` | `qrtz_triggers` | Quartz.NET 3.19.0 |

Upgrading from the 3.17.x package generates a migration that adds these columns.
All three are nullable or defaulted, so the migration is safe to apply to a live
scheduler before rolling out the matching Quartz.NET upgrade.

### 3.20.x realigns the indexes

3.20.0 changes no table and no column. It tracks [Quartz.NET
3.20.0's PostgreSQL index realignment](https://github.com/quartznet/quartznet/blob/v3.20.0/database/migrations/3.20/index_alignment_postgres.sql):
every index now leads with `sched_name`, which every `AdoJobStore` statement filters on
first, and `idx_qrtz_t_nft_st` gets its columns in acquire-query order
(`sched_name, trigger_state, next_fire_time`) instead of the reversed
`next_fire_time, trigger_state`. Eleven indexes become ten — seven single-column indexes
that no statement could drive a scan from are replaced by five composites.

Upgrading from the 3.19.x package generates a migration that drops the old indexes and
creates the new ones. Two things worth knowing before applying it:

- **It is performance-only.** Quartz.NET never names an index, so nothing breaks on any
  version if you skip it, and a 3.20.x package is safe against a 3.18.x or 3.19.x
  scheduler.
- **On a busy database, hand-edit the generated migration** to use
  `CREATE INDEX CONCURRENTLY` / `DROP INDEX CONCURRENTLY` and run those statements
  outside a transaction. EF Core wraps a migration in one by default, and neither
  concurrent form can run inside a transaction block.

## License

MIT - see [LICENSE](https://github.com/canopy-labs/Quartz.EntityFramework.Migrations/blob/master/LICENSE) for details.

Copyright (c) Canopy Labs
