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
| 3.19.x | 3.18.x, 3.19.x | 8, 9, 10 |
| 3.17.x | 3.17.x | 8, 9, 10 |

Match the major.minor version of this package to your Quartz.NET version.

Each schema change is additive, so a newer package works with an older Quartz.NET —
the reverse does not. 3.19.x adds three columns on top of 3.17.x:

| Column | Table | Added in |
|---|---|---|
| `execution_group` | `qrtz_triggers`, `qrtz_fired_triggers` | Quartz.NET 3.18.0 |
| `preferred_node` | `qrtz_triggers` | Quartz.NET 3.19.0 |
| `preferred_node_auto` | `qrtz_triggers` | Quartz.NET 3.19.0 |

Upgrading from the 3.17.x package generates a migration that adds these columns.
All three are nullable or defaulted, so the migration is safe to apply to a live
scheduler before rolling out the matching Quartz.NET upgrade.

## License

MIT - see [LICENSE](LICENSE) for details.

Copyright (c) Canopy Labs
