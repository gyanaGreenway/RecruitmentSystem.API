using System.Data.Common;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecruitmentSystem.API.Data;

namespace RecruitmentSystem.API.Database;

public static class StartupSqlScriptsRunner
{
    public static async Task RunAsync(ApplicationDbContext context, string contentRootPath, ILogger logger, CancellationToken cancellationToken = default)
    {
        // Ensure tracking table exists
        const string ensureTable = @"IF OBJECT_ID('dbo.SchemaVersions','U') IS NULL
                                    BEGIN
                                     CREATE TABLE dbo.SchemaVersions(
                                     Id int IDENTITY(1,1) PRIMARY KEY,
                                     ScriptName nvarchar(260) NOT NULL,
                                     AppliedAt datetime2 NOT NULL DEFAULT SYSUTCDATETIME()
                                     );
                                    END";
        await context.Database.ExecuteSqlRawAsync(ensureTable, cancellationToken);

        var scriptsDir = Path.Combine(contentRootPath, "Database", "Scripts");
        if (!Directory.Exists(scriptsDir))
        {
            logger.LogInformation("No SQL scripts directory found at {Dir}", scriptsDir);
            return;
        }

        foreach (var file in Directory.EnumerateFiles(scriptsDir, "*.sql", SearchOption.TopDirectoryOnly)
        .OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
        {
            var name = Path.GetFileName(file);
            if (await IsAppliedAsync(context, name, cancellationToken))
            {
                logger.LogInformation("Skipping already applied script {Script}", name);
                continue;
            }

            var script = await File.ReadAllTextAsync(file, cancellationToken);
            var batches = SplitBatches(script);
            logger.LogInformation("Applying script {Script} with {BatchCount} batches", name, batches.Count);

            using var tx = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                foreach (var batch in batches)
                {
                    if (string.IsNullOrWhiteSpace(batch)) continue;
                    await context.Database.ExecuteSqlRawAsync(batch, cancellationToken);
                }

                await context.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO dbo.SchemaVersions (ScriptName) VALUES ({name})", cancellationToken);
                await tx.CommitAsync(cancellationToken);
                logger.LogInformation("Applied script {Script}", name);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(cancellationToken);
                logger.LogError(ex, "Failed applying script {Script}", name);
                throw;
            }
        }
    }

    private static async Task<bool> IsAppliedAsync(ApplicationDbContext context, string scriptName, CancellationToken ct)
    {
        await using var conn = context.Database.GetDbConnection();
        if (conn.State != System.Data.ConnectionState.Open)
            await conn.OpenAsync(ct);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = @"IF OBJECT_ID('dbo.SchemaVersions','U') IS NULL SELECT -1 ELSE SELECT COUNT(1) FROM dbo.SchemaVersions WHERE ScriptName = @name";
        var p = cmd.CreateParameter();
        p.ParameterName = "@name";
        p.Value = scriptName;
        cmd.Parameters.Add(p);
        var result = (int)(await cmd.ExecuteScalarAsync(ct) ?? 0);
        return result > 0;
    }

    private static List<string> SplitBatches(string sql)
    {
        var batches = new List<string>();
        var sb = new StringBuilder();
        using var reader = new StringReader(sql);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (line.Trim().Equals("GO", StringComparison.OrdinalIgnoreCase))
            {
                batches.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.AppendLine(line);
            }
        }
        if (sb.Length > 0)
        {
            batches.Add(sb.ToString());
        }
        return batches;
    }
}
