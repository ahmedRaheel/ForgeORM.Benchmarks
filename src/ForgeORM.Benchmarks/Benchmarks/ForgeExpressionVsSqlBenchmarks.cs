using BenchmarkDotNet.Attributes;
using ForgeORM.Benchmarks.Forge;
using ForgeORM.Benchmarks.Infrastructure;
using ForgeORM.Benchmarks.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeORM.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks ForgeORM expression/queryable style versus SQL/raw style.
/// Matches the currently available methods in ForgeExpressionMethodBenchmarks.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class ForgeExpressionVsSqlBenchmarks
{
    private ServiceProvider _provider = default!;
    private ForgeExpressionMethodBenchmarks _forge = default!;

    [GlobalSetup]
    public void Setup()
    {
        _provider = BenchmarkServices.Build();
        _forge = _provider.GetRequiredService<ForgeExpressionMethodBenchmarks>();

        _forge.Setup();
    }

    [Benchmark(Baseline = true)]
    public async Task<IReadOnlyList<Order>> Expression_Where_ToList()
    {
        return await _forge.Expression_Where_ToListAsync();
    }

    [Benchmark]
    public async Task<IReadOnlyList<Order>> Sql_Where_ToList()
    {
        return  await _forge.Sql_Where_ToListAsync();
    }

    [Benchmark]
    public async Task<Order?> Expression_FirstOrDefault()
    {
        return  await _forge.Expression_FirstOrDefaultAsync();
    }

    [Benchmark]
    public  async  Task<Order?> Sql_FirstOrDefault()
    {
        return await _forge.Sql_FirstOrDefaultAsync();
    }

    [Benchmark]
    public async Task<int> Expression_Count()
    {
        return await _forge.Expression_CountAsync();
    }

    [Benchmark]
    public  async Task<int> Sql_Count()
    {
        return await  _forge.Sql_CountAsync();
    }

    [Benchmark]
    public Task<bool> Expression_Any()
    {
        return _forge.Expression_AnyAsync();
    }

    [Benchmark]
    public async Task<bool> Sql_Any()
    {
        return await _forge.Sql_AnyAsync();
    }

    [Benchmark]
    public async Task<IReadOnlyList<Order>> Expression_Where_OrderByDescending_Skip_Take_ToList()
    {
        return await _forge.Expression_Where_OrderByDescending_Skip_Take_ToListAsync();
    }

    [Benchmark]
    public async Task<IReadOnlyList<Order>> Sql_Where_OrderByDescending_Skip_Take_ToList()
    {
        return await _forge.Sql_Where_OrderByDescending_Skip_Take_ToListAsync();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _provider.Dispose();
    }
}