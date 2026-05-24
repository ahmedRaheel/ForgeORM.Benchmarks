using BenchmarkDotNet.Attributes;
using ForgeORM.Benchmarks.Infrastructure;
using ForgeORM.Benchmarks.Models;
using ForgeORM.Core;

namespace ForgeORM.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks the EF-style ForgeORM expression/queryable API.
/// Child/reference split-query loading is intentionally not enabled here so parent-only hot paths stay measurable.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class ForgeExpressionQueryBenchmarks
{
    private ForgeDbContext _db = default!;
    private BenchmarkSettings _settings = default!;

    [Params(10, 50, 100)]
    public int Take { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _settings = new BenchmarkSettings();
        _db = ForgeBenchmarkDbFactory.Create(_settings.ConnectionString);
    }

    [Benchmark(Baseline = true)]
    public async Task<IReadOnlyList<Order>> Where_ToListAsync()
    {
        return await _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .ToListAsync();
    }

    [Benchmark]
    public async Task<Order?> Where_FirstOrDefaultAsync()
    {
        return await _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .FirstOrDefaultAsync();
    }

    [Benchmark]
    public async Task<bool> Where_AnyAsync()
    {
        return await _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .AnyAsync();
    }

    [Benchmark]
    public async Task<int> Where_CountAsync()
    {
        return await _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .CountAsync();
    }

    [Benchmark]
    public async Task<decimal> Where_SumAsync()
    {
        return await _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .SumAsync(x => x.GrandTotal);
    }

    [Benchmark]
    public async Task<decimal> Where_AverageAsync()
    {
        return await _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .AverageAsync(x => x.GrandTotal);
    }

    [Benchmark]
    public async Task<decimal> Where_MinAsync()
    {
        return await _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .MinAsync(x => x.GrandTotal);
    }

    [Benchmark]
    public async Task<decimal> Where_MaxAsync()
    {
        return await _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .MaxAsync(x => x.GrandTotal);
    }

    [Benchmark]
    public async Task<IReadOnlyList<Order>> Where_OrderByDescending_Skip_Take_ToListAsync()
    {
        return await _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .OrderByDescending(x => x.Id)
            .Skip(0)
            .Take(Take)
            .ToListAsync();
    }

    //[Benchmark]
    //public Task<ForgePagedResult<Order>> Where_PageAsync()
    //{
    //    return _db.Set<Order>()
    //        .Where(x => x.CustomerId == _settings.QueryCustomerId)
    //        .OrderByDescending(x => x.Id)
    //        .PageAsync(page: 1, pageSize: Take);
    //}

    [Benchmark]
    public async Task<IReadOnlyList<Order>> WhereSql_OrderBySql_ToListAsync()
    {
        return await _db.Set<Order>()
            .WhereSql("CustomerId = @CustomerId", new { CustomerId = _settings.QueryCustomerId })
            .OrderBySql("Id DESC")
            .Take(Take)
            .ToListAsync();
    }

    [Benchmark]
    public async Task<IReadOnlyList<Order>> WhereIf_WhereSqlIf_ToListAsync()
    {
        return await _db.Set<Order>()
            .WhereIf(true, x => x.CustomerId == _settings.QueryCustomerId)
            .WhereSqlIf(false, "Status = @Status", new { Status = "Paid" })
            .OrderByDescending(x => x.Id)
            .Take(Take)
            .ToListAsync();
    }
}
