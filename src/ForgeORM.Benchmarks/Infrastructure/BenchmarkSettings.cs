using Microsoft.Extensions.Configuration;

namespace ForgeORM.Benchmarks.Infrastructure;

public sealed class BenchmarkSettings
{
    public string ConnectionString { get; init; } = string.Empty;
    public int SeedCustomers { get; init; } = 1000;
    public int SeedOrdersPerCustomer { get; init; } = 20;
    public int QueryCustomerId { get; init; } = 10;
    public int QueryOrderId { get; init; } = 100;

    public static BenchmarkSettings Load()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        return new BenchmarkSettings
        {
            ConnectionString = configuration["Benchmark:ConnectionString"]
                ?? throw new InvalidOperationException("Benchmark:ConnectionString is missing."),
            SeedCustomers = ReadInt(configuration, "Benchmark:SeedCustomers", 1000),
            SeedOrdersPerCustomer = ReadInt(configuration, "Benchmark:SeedOrdersPerCustomer", 20),
            QueryCustomerId = ReadInt(configuration, "Benchmark:QueryCustomerId", 10),
            QueryOrderId = ReadInt(configuration, "Benchmark:QueryOrderId", 100)
        };
    }

    private static int ReadInt(IConfiguration configuration, string key, int defaultValue)
    {
        var value = configuration[key];
        return int.TryParse(value, out var result) ? result : defaultValue;
    }
}
