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
    public Task<IReadOnlyList<Order>> Where_ToListAsync()
    {
        return _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .ToListAsync(includeChildren: false);
    }

    [Benchmark]
    public Task<Order?> Where_FirstOrDefaultAsync()
    {
        return _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .FirstOrDefaultAsync(includeChildren:false);
    }

    [Benchmark]
    public Task<bool> Where_AnyAsync()
    {
        return _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .AnyAsync();
    }

    [Benchmark]
    public Task<int> Where_CountAsync()
    {
        return _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .CountAsync();
    }

    [Benchmark]
    public Task<decimal> Where_SumAsync()
    {
        return _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .SumAsync(x => x.GrandTotal);
    }

    [Benchmark]
    public Task<decimal> Where_AverageAsync()
    {
        return _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .AverageAsync(x => x.GrandTotal);
    }

    [Benchmark]
    public Task<decimal> Where_MinAsync()
    {
        return _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .MinAsync(x => x.GrandTotal);
    }

    [Benchmark]
    public Task<decimal> Where_MaxAsync()
    {
        return _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .MaxAsync(x => x.GrandTotal);
    }

    [Benchmark]
    public Task<IReadOnlyList<Order>> Where_OrderByDescending_Skip_Take_ToListAsync()
    {
        return _db.Set<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .OrderByDescending(x => x.Id)
            .Skip(0)
            .Take(Take)
            .ToListAsync(includeChildren: false);
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
    public Task<IReadOnlyList<Order>> WhereSql_OrderBySql_ToListAsync()
    {
        return _db.Set<Order>()
            .WhereSql("CustomerId = @CustomerId", new { CustomerId = _settings.QueryCustomerId })
            .OrderBySql("Id DESC")
            .Take(Take)
            .ToListAsync(includeChildren: false);
    }

    [Benchmark]
    public Task<IReadOnlyList<Order>> WhereIf_WhereSqlIf_ToListAsync()
    {
        return _db.Set<Order>()
            .WhereIf(true, x => x.CustomerId == _settings.QueryCustomerId)
            .WhereSqlIf(false, "Status = @Status", new { Status = "Paid" })
            .OrderByDescending(x => x.Id)
            .Take(Take)
            .ToListAsync(includeChildren: false);
    }
}
