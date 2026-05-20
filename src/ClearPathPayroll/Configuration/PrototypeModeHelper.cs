using Microsoft.Extensions.Hosting;

namespace ClearPathPayroll.Configuration;

public static class PrototypeModeHelper
{
    public static bool ShouldUseLocalPrototypeMode(IHostEnvironment environment, PrototypeModeOptions options)
    {
        if (environment is null)
        {
            throw new ArgumentNullException(nameof(environment));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        // Always use local prototype mode in development.
        if (environment.IsDevelopment())
        {
            return true;
        }

        // Allow local prototype mode in other non-production environments when explicitly enabled.
        return !environment.IsProduction() && options.Enabled;
    }

    public static string GetLocalDbConnectionString(string databaseName)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException("Database name is required.", nameof(databaseName));
        }

        return $"Server=(localdb)\\mssqllocaldb;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true";
    }
}
