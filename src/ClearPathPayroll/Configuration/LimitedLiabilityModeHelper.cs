using Microsoft.Extensions.Hosting;

namespace ClearPathPayroll.Configuration;

public static class LimitedLiabilityModeHelper
{
    public const string SqlServerLocalDbProvider = "SqlServerLocalDb";
    public const string SqliteProvider = "SQLite";

    public static bool ShouldUseLocalOnlyMode(
        IHostEnvironment environment,
        LimitedLiabilityModeOptions options,
        bool localPrototypeEnabled)
    {
        if (environment is null)
        {
            throw new ArgumentNullException(nameof(environment));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return environment.IsDevelopment() || localPrototypeEnabled || options.Enabled;
    }

    public static void ValidateStartupSafety(LimitedLiabilityModeOptions options, bool localDemoMode)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (string.IsNullOrWhiteSpace(options.LocalDatabaseProvider))
        {
            throw new InvalidOperationException("LimitedLiabilityMode:LocalDatabaseProvider is required.");
        }

        if (string.IsNullOrWhiteSpace(options.LocalDatabaseName))
        {
            throw new InvalidOperationException("LimitedLiabilityMode:LocalDatabaseName is required.");
        }

        if (!IsSupportedLocalDatabaseProvider(options.LocalDatabaseProvider))
        {
            throw new InvalidOperationException("LimitedLiabilityMode:LocalDatabaseProvider must be SqlServerLocalDb or SQLite.");
        }

        if (!localDemoMode)
        {
            return;
        }

        if (options.AllowTelemetry)
        {
            throw new InvalidOperationException("Telemetry cannot be enabled in local demo mode.");
        }

        if (options.AllowRealAchSubmission)
        {
            throw new InvalidOperationException("Real ACH submission cannot be enabled in local demo mode.");
        }

        if (options.AllowRealTaxFiling)
        {
            throw new InvalidOperationException("Real tax filing cannot be enabled in local demo mode.");
        }
    }

    public static string GetLocalDbConnectionString(string databaseName)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException("Database name is required.", nameof(databaseName));
        }

        return $"Server=(localdb)\\mssqllocaldb;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true";
    }

    public static string GetSqliteConnectionString(IHostEnvironment environment, string databaseName)
    {
        if (environment is null)
        {
            throw new ArgumentNullException(nameof(environment));
        }

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException("Database name is required.", nameof(databaseName));
        }

        var safeDatabaseName = databaseName.EndsWith(".db", StringComparison.OrdinalIgnoreCase)
            ? databaseName
            : $"{databaseName}.db";
        var dataDirectory = Path.Combine(environment.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dataDirectory);

        return $"Data Source={Path.Combine(dataDirectory, safeDatabaseName)}";
    }

    public static bool IsSqlServerLocalDb(string provider)
    {
        return string.Equals(provider, SqlServerLocalDbProvider, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsSqlite(string provider)
    {
        return string.Equals(provider, SqliteProvider, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSupportedLocalDatabaseProvider(string provider)
    {
        return IsSqlServerLocalDb(provider) ||
               string.Equals(provider, SqliteProvider, StringComparison.OrdinalIgnoreCase);
    }
}
