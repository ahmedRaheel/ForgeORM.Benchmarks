using ForgeORM.Abstractions;
using ForgeORM.Analytics;
using ForgeORM.Benchmarks.Infrastructure;
using ForgeORM.Core;
using ForgeORM.Providers.SqlServer;

namespace ForgeORM.Benchmarks.Forge;

/// <summary>
/// Central place to create one cached ForgeDbContext for benchmark readers.
/// Keep this factory reflection/runtime-emit only; do not create a context inside benchmark methods.
/// </summary>
public static class ForgeDbContextFactory
{
    public static ForgeDbContext Create()
    {
        var settings = BenchmarkSettings.Load();

        IForgeDatabaseProvider provider = new SqlServerForgeProvider();
        IForgeEntityMetadataResolver metadata = new ReflectionForgeEntityMetadataResolver();
        IForgeQueryAnalyzer analyzer = new BasicForgeQueryAnalyzer();

        return new ForgeDbContext(
            settings.ConnectionString,
            provider,
            metadata,
            analyzer);
    }
}
