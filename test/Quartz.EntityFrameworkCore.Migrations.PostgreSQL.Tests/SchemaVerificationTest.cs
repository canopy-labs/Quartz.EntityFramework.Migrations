using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Quartz.EntityFrameworkCore.Migrations.PostgreSQL.Tests;

public class SchemaVerificationTest
{
    private static string GetOfficialScriptUrl()
    {
        var version = typeof(ModelBuilderExtensions).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
            .InformationalVersion;

        // Strip any suffix (e.g. "3.17.0+sha" or "3.17.0-preview")
        var parts = version.Split(['+', '-'], 2);
        var semver = new Version(parts[0]);

        return $"https://raw.githubusercontent.com/quartznet/quartznet/v{semver.Major}.{semver.Minor}.0/database/tables/tables_postgres.sql";
    }

    [Fact]
    public async Task GeneratedSchema_MatchesOfficialScript()
    {
        // Fetch the official SQL script
        var officialScriptUrl = GetOfficialScriptUrl();
        using var httpClient = new HttpClient();
        var officialSql = await httpClient.GetStringAsync(officialScriptUrl);

        // Generate migration SQL from EF model
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseNpgsql("Host=localhost") // connection not actually opened
            .Options;

        using var context = new TestDbContext(options);
        var migrationSql = context.Database.GenerateCreateScript();

        // Parse both scripts into comparable structures
        var officialTables = ParseCreateTables(officialSql);
        var generatedTables = ParseCreateTables(migrationSql);

        // Compare table names
        var officialTableNames = officialTables.Keys.OrderBy(k => k).ToList();
        var generatedTableNames = generatedTables.Keys.OrderBy(k => k).ToList();
        Assert.Equal(officialTableNames, generatedTableNames);

        // Compare columns per table
        foreach (var tableName in officialTableNames)
        {
            var officialColumns = officialTables[tableName];
            var generatedColumns = generatedTables[tableName];

            Assert.True(
                officialColumns.SequenceEqual(generatedColumns),
                $"Column mismatch in table '{tableName}'.\n" +
                $"Official:  [{string.Join(", ", officialColumns)}]\n" +
                $"Generated: [{string.Join(", ", generatedColumns)}]");
        }

        // Compare indexes
        var officialIndexes = ParseCreateIndexes(officialSql).OrderBy(i => i).ToList();
        var generatedIndexes = ParseCreateIndexes(migrationSql).OrderBy(i => i).ToList();
        Assert.Equal(officialIndexes, generatedIndexes);
    }

    private static Dictionary<string, List<string>> ParseCreateTables(string sql)
    {
        var tables = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        var tablePattern = new System.Text.RegularExpressions.Regex(
            @"CREATE\s+TABLE\s+(\w+)\s*\((.*?)\);",
            System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        foreach (System.Text.RegularExpressions.Match match in tablePattern.Matches(sql))
        {
            var tableName = match.Groups[1].Value.ToLowerInvariant();
            var body = match.Groups[2].Value;
            var columns = new List<string>();

            foreach (var line in body.Split('\n'))
            {
                var trimmed = line.Trim().TrimEnd(',');
                if (string.IsNullOrWhiteSpace(trimmed)) continue;
                if (trimmed.StartsWith("PRIMARY", StringComparison.OrdinalIgnoreCase)) continue;
                if (trimmed.StartsWith("FOREIGN", StringComparison.OrdinalIgnoreCase)) continue;
                if (trimmed.StartsWith("REFERENCES", StringComparison.OrdinalIgnoreCase)) continue;
                if (trimmed.StartsWith("ON ", StringComparison.OrdinalIgnoreCase)) continue;
                if (trimmed.StartsWith("CONSTRAINT", StringComparison.OrdinalIgnoreCase)) continue;

                var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    var colName = parts[0].ToLowerInvariant();
                    var colType = parts[1].ToLowerInvariant();
                    var nullable = trimmed.Contains("NOT NULL", StringComparison.OrdinalIgnoreCase) ? "NOT NULL" : "NULL";
                    columns.Add($"{colName} {colType} {nullable}");
                }
            }

            if (columns.Count > 0)
                tables[tableName] = columns;
        }

        return tables;
    }

    private static List<string> ParseCreateIndexes(string sql)
    {
        var indexes = new List<string>();
        var indexPattern = new System.Text.RegularExpressions.Regex(
            @"CREATE\s+INDEX\s+(\w+)\s+ON\s+(\w+)\s*\(([^)]+)\)",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        foreach (System.Text.RegularExpressions.Match match in indexPattern.Matches(sql))
        {
            var indexName = match.Groups[1].Value.ToLowerInvariant();
            var tableName = match.Groups[2].Value.ToLowerInvariant();
            var columns = string.Join(", ", match.Groups[3].Value
                .Split(',')
                .Select(c => c.Trim().ToLowerInvariant()));
            indexes.Add($"{indexName} ON {tableName}({columns})");
        }

        return indexes;
    }
}
