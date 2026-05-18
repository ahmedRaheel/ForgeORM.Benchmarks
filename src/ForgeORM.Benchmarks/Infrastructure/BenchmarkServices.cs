using ForgeORM.Benchmarks.Dapper;
using ForgeORM.Benchmarks.Ef;
using ForgeORM.Benchmarks.Forge;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeORM.Benchmarks.Infrastructure;

public static class BenchmarkServices
{
    public static ServiceProvider Build()
    {
        var settings = BenchmarkSettings.Load();

        var services = new ServiceCollection();
        services.AddSingleton(settings);
        services.AddSingleton(new SqlConnectionFactory(settings.ConnectionString));
        services.AddSingleton<DatabaseSeeder>();
        services.AddSingleton<DapperOrderReader>();
        services.AddSingleton(new ForgeOrderReader(settings.ConnectionString));
        services.AddSingleton<ForgeExpressionMethodBenchmarks>();

        services.AddPooledDbContextFactory<BenchmarkDbContext>(options =>
        {
            options.UseSqlServer(settings.ConnectionString);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            options.EnableSensitiveDataLogging(false);
            options.EnableDetailedErrors(false);
        });
        services.AddSingleton<EfOrderReader>();

        return services.BuildServiceProvider(validateScopes: true);
    }
}
