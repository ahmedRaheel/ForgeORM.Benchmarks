using ForgeORM.Abstractions;
using ForgeORM.Core;
using ForgeORM.Providers.SqlServer;

namespace ForgeORM.Benchmarks.Infrastructure;

public static class ForgeBenchmarkDbFactory
{
    public static ForgeDbContext Create()
    {
        var settings = new BenchmarkSettings();
        return Create(settings.ConnectionString);
    }

    public static ForgeDbContext Create(string connectionString)
    {
        IForgeDatabaseProvider provider = new SqlServerForgeProvider();
        IForgeEntityMetadataResolver metadata = new ReflectionForgeEntityMetadataResolver();
        IForgeQueryAnalyzer analyzer = new NoopForgeQueryAnalyzer();

        return new ForgeDbContext(connectionString, provider, metadata, analyzer);
    }
}
