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
public class SearchAndPagingBenchmarks
{
    private ServiceProvider _provider = default!;
    private BenchmarkSettings _settings = default!;
    private DapperOrderReader _dapper = default!;
    private EfOrderReader _ef = default!;
    private ForgeOrderReader _forge = default!;

    [Params(10, 50, 100)]
    public int Take { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _provider = BenchmarkServices.Build();
        _settings = _provider.GetRequiredService<BenchmarkSettings>();
        _dapper = _provider.GetRequiredService<DapperOrderReader>();
        _ef = _provider.GetRequiredService<EfOrderReader>();
        _forge = _provider.GetRequiredService<ForgeOrderReader>();
    }

    [Benchmark(Baseline = true)]
    public Task<IReadOnlyList<OrderDto>> Dapper_Search_Paged() =>
        _dapper.SearchPagedAsync(_settings.QueryCustomerId, skip: 0, take: Take);

    [Benchmark]
    public async Task<IReadOnlyList<OrderDto>> EF_Core_Search_Paged() =>
        await _ef.SearchPagedAsync(_settings.QueryCustomerId, skip: 0, take: Take);

    [Benchmark]
    public Task<IReadOnlyList<OrderDto>> ForgeORM_Search_Paged() =>
        _forge.SearchPagedAsync(_settings.QueryCustomerId, skip: 0, take: Take);

    [GlobalCleanup]
    public void Cleanup() => _provider.Dispose();
}
