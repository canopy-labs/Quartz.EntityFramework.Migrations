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
| 4.3.x | 4.0.x, 4.1.x, 4.2.x, 4.3.x | 8, 9, 10 |
| 4.2.x | 4.0.x, 4.1.x, 4.2.x | 8, 9, 10 |
| 4.0.x | 4.0.x, 4.1.x | 8, 9, 10 |
| 3.20.x | 3.18.x, 3.19.x, 3.20.x | 8, 9, 10 |
| 3.19.x | 3.18.x, 3.19.x | 8, 9, 10 |
| 3.17.x | 3.17.x | 8, 9, 10 |

Match the major.minor version of this package to your Quartz.NET version.

A newer package works with an older Quartz.NET; the reverse does not. Every column
added so far is nullable or defaulted, and Quartz.NET 3.18+ probes for the columns it
needs rather than requiring them, so a column a given Quartz.NET version does not know
about is simply unused.

**Quartz.NET 4.0 makes the reverse direction fail loudly.** It removed those probes,
assumes `misfire_orig_fire_time`, `execution_group`, `preferred_node` and
`preferred_node_auto` all exist, and validates its schema at startup. A 3.x-era package
under a 4.x scheduler no longer degrades quietly — it stops the scheduler.

3.19.x added three columns on top of 3.17.x:

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

### 4.0.x tracks the Quartz.NET 4.0 schema

4.0.0 is the first release to add a table. It tracks [Quartz.NET 4.0's mandatory
migration](https://github.com/quartznet/quartznet/blob/v4.0.0/database/migrations/4.0/schema_30_to_40_upgrade_postgres.sql):

| Change | Object | Why |
|---|---|---|
| `qrtz_paused_job_grps` (new table) | — | 3.x paused a job group without recording it, so a paused group could not be listed or survive a restart. 4.x keeps the group names here. |
| `retry_policy varchar(250) null` | `qrtz_triggers` | The trigger's retry policy. |
| `retry_attempt integer null` | `qrtz_triggers` | Retries already made for the occurrence being executed. |
| `idx_qrtz_t_nft_st` reshaped | `qrtz_triggers` | Now `(sched_name, trigger_state, next_fire_time ASC, priority DESC, misfire_instr)` — the order acquisition reads in. |
| `idx_qrtz_t_next_fire_time` dropped | `qrtz_triggers` | Redundant: the reshaped `idx_qrtz_t_nft_st` is a covering superset. |
| `idx_qrtz_j_req_recovery` dropped | `qrtz_job_details` | No 4.x statement can drive a scan from it. |

Both new columns are nullable with no default, so every existing row reads as "no retry
policy" and no data migration is needed. Upgrading from the 3.20.x package generates a
migration that adds the table and columns and reshapes the indexes.

Two things worth knowing before applying it:

- **The column half is safe during a rolling upgrade; the index half is not.** Upstream
  ships these as two scripts for that reason. The generated EF migration combines them.
  If you are rolling 3.x and 4.x nodes side by side, split the generated migration and
  hold the index statements back until the last 3.x node is down.
- **On a busy database, hand-edit the generated migration** to use
  `CREATE INDEX CONCURRENTLY` / `DROP INDEX CONCURRENTLY` and run those statements
  outside a transaction. EF Core wraps a migration in one by default, and neither
  concurrent form can run inside a transaction block.

### 4.2.x tracks the Quartz.NET 4.2 schema

Quartz.NET 4.1 changed no table, so there is no 4.1.x package: 4.0.x covers 4.1.x.
4.2.0 adds three columns and two tables, taken from upstream's two
[4.2 migrations](https://github.com/quartznet/quartznet/tree/v4.2.0/database/migrations/4.2):

| Change | Object | Required by Quartz.NET 4.2? |
|---|---|---|
| `continues_trigger_name text null` | `qrtz_triggers` | **Yes.** Names the trigger a continuation waits on. |
| `continues_trigger_group text null` | `qrtz_triggers` | **Yes.** Its group. |
| `continuation_condition integer null` | `qrtz_triggers` | **Yes.** The `ContinuationCondition` flags that release the wait. |
| `qrtz_execution_history` (new table) + 2 indexes | — | No. Read and written only when `UseExecutionHistory()` is configured. |
| `qrtz_misfire_history` (new table) + 2 indexes | — | No. Same condition. |

**A 4.2 scheduler refuses to start without the three continuation columns.** It
validates its schema at startup, so a 4.0.x package under Quartz.NET 4.2 stops the
scheduler with `column "continues_trigger_name" does not exist`.

Upgrading from the 4.0.x package generates a migration that adds the columns, the two
tables and their indexes. It is safe to apply before rolling out Quartz.NET 4.2:

- The three columns are nullable with no default, so existing rows need no data
  migration, and 4.0/4.1 nodes never read them. Migrate, roll every node to 4.2, and
  only then schedule continuations, since a 4.0 or 4.1 node cannot settle one.
- The history tables have no foreign keys and nothing else references them, so they are
  inert until a 4.2 node turns the history on.

### 4.3.x tracks the Quartz.NET 4.3 schema

4.3.0 adds columns only: no table and no index changes. It follows upstream's five
[4.3 migrations](https://github.com/quartznet/quartznet/tree/v4.3.0/database/migrations/4.3):

| Change | Object | Required by Quartz.NET 4.3? |
|---|---|---|
| `overlap_policy integer null` | `qrtz_triggers` | **Yes.** What to do when a firing lands while the previous one is still running. |
| `pause_reason varchar(250) null`, `paused_by varchar(200) null`, `paused_at bigint null` | `qrtz_triggers`, `qrtz_paused_trigger_grps`, `qrtz_paused_job_grps` | **Yes.** Who paused the trigger or group, when, and why. |
| `progress integer null`, `progress_message varchar(250) null` | `qrtz_fired_triggers` | **Yes.** What a running job last reported about its progress. |
| `execution_log text null` | `qrtz_execution_history` | No. Only used with `UseExecutionHistory()`. |
| `reason integer null` | `qrtz_misfire_history` | No. Same condition. |

**A 4.3 scheduler refuses to start without the required columns**, so a 4.2.x package
under Quartz.NET 4.3 stops the scheduler.

Upgrading from the 4.2.x package generates a migration that only adds nullable columns
with no default. Existing rows need no data migration, and 4.0 to 4.2 nodes never read
the new columns, so it is safe to apply before rolling out Quartz.NET 4.3.

## License

MIT - see [LICENSE](https://github.com/canopy-labs/Quartz.EntityFramework.Migrations/blob/master/LICENSE) for details.

Copyright (c) Canopy Labs
