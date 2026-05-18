using ForgeORM.Abstractions;
using ForgeORM.Analytics;
using ForgeORM.Benchmarks.Infrastructure;
using ForgeORM.Core;
using ForgeORM.Providers.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeORM.Benchmarks.Forge;

/// <summary>
/// Central place to create ForgeDbContext for benchmarks.
/// Your ForgeDbContext is not parameterless, so benchmarks must create it through options/DI here.
/// </summary>
public static class ForgeDbContextFactory
{
    /// <summary>
    /// Create a ForgeDbContext using your real ForgeORM registration.
    /// Replace the placeholder body after adding the real ForgeORM package references.
    /// </summary>
    public static ForgeDbContext Create()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            //.AddEnvironmentVariables()
            .Build();

        var settings = BenchmarkSettings.Load();

        IForgeDatabaseProvider provider =
            new SqlServerForgeProvider();

        IForgeEntityMetadataResolver metadata =
            new ReflectionForgeEntityMetadataResolver();

        IForgeQueryAnalyzer analyzer =
            new BasicForgeQueryAnalyzer();

        return new ForgeDbContext(
            settings.ConnectionString,
            provider,
            metadata,
            analyzer);
    }
}
