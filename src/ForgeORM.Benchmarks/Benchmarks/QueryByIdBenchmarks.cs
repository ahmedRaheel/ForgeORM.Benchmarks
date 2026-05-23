using BenchmarkDotNet.Attributes;
using ForgeORM.Benchmarks.Dapper;
using ForgeORM.Benchmarks.Ef;
using ForgeORM.Benchmarks.Forge;
using ForgeORM.Benchmarks.Infrastructure;
using ForgeORM.Benchmarks.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeORM.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class QueryByIdBenchmarks
{
    private ServiceProvider _provider = default!;
    private BenchmarkSettings _settings = default!;
    private DapperOrderReader _dapper = default!;
    private EfOrderReader _ef = default!;
    private ForgeOrderReader _forge = default!;

    [GlobalSetup]
    public async Task Setup()
    {
        _provider = BenchmarkServices.Build();
        _settings = _provider.GetRequiredService<BenchmarkSettings>();
        _dapper = _provider.GetRequiredService<DapperOrderReader>();
        _ef = _provider.GetRequiredService<EfOrderReader>();
        _forge = _provider.GetRequiredService<ForgeOrderReader>();
        await _provider.GetRequiredService<DatabaseSeeder>().RecreateAndSeedAsync();
    }

    [Benchmark(Baseline = true)]
    public async Task<OrderDto?> Dapper_Query_By_Id() => await _dapper.GetByIdAsync(_settings.QueryOrderId);

    [Benchmark]
    public async Task<OrderDto?> EF_Core_Query_By_Id() => await _ef.GetByIdAsync(_settings.QueryOrderId);

    [Benchmark]
    public async Task<Order> ForgeORM_Query_By_Id() => await _forge.GetByIdAsync(_settings.QueryOrderId);

    [GlobalCleanup]
    public void Cleanup() => _provider.Dispose();
}
