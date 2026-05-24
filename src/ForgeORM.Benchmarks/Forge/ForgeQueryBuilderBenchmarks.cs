using BenchmarkDotNet.Attributes;
using ForgeORM.Benchmarks.Infrastructure;
using ForgeORM.Benchmarks.Models;
using ForgeORM.Core;

namespace ForgeORM.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks ForgeORM.Core ForgeQueryBuilder APIs such as Select, Where, joins, paging, rendering, validation and execution.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class ForgeQueryBuilderBenchmarks
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
    public string QueryBuilder_Render_Select_Where_Order_Page()
    {
        return _db.Query<Order>()
            .Select(x => x.Id, x => x.CustomerId, x => x.OrderNo, x => x.GrandTotal)
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .OrderByDescending(x => x.Id)
            .Take(Take)
            .ToSql();
    }

    [Benchmark]
    public async Task<IReadOnlyList<Order>> QueryBuilder_Execute_ToListAsync()
    {
        return await _db.Query<Order>()
            .Select(x => x.Id, x => x.CustomerId, x => x.OrderNo, x => x.Status, x => x.GrandTotal,  x => x.CreatedAt, x => x.OrderDate)
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .OrderByDescending(x => x.Id)
            .Take(Take)
            .ToListAsync();
    }

    [Benchmark]
    public string QueryBuilder_Render_Join_Projection()
    {
        return _db.Query<Order>()
            .From("Orders o")
            .SelectSql("o.Id", "o.OrderNo", "o.CustomerId", "o.Status", "o.GrandTotal")
            .InnerJoin("Customers c", "c.Id = o.CustomerId")
            .WhereSql("o.CustomerId = @CustomerId", new { CustomerId = _settings.QueryCustomerId })
            .OrderByDescending(x => x.Id)
            .Take(Take)
            .ToSql();
    }

    [Benchmark]
    public string QueryBuilder_Render_WhereExists_CaseWhen()
    {
        return _db.Query<Order>()
            .Select(x => x.Id, x => x.CustomerId, x => x.OrderNo)
            .CaseWhen("GrandTotal >= 1000", "1", "0", "IsLargeOrder")
            .WhereExists("SELECT 1 FROM OrderItems oi WHERE oi.OrderId = Orders.Id")
            .OrderByDescending(x => x.Id)
            .Take(Take)
            .ToSql();
    }

    [Benchmark]
    public ForgeQueryValidationResult QueryBuilder_Validate()
    {
        return _db.Query<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .Take(Take)
            .Validate();
    }

    [Benchmark]
    public ForgeDebugSql QueryBuilder_ToDebugSql()
    {
        return _db.Query<Order>()
            .Tag("benchmark")
            .Comment("ForgeORM query builder benchmark")
            .Select(x => x.Id, x => x.CustomerId, x => x.OrderNo)
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .OrderByDescending(x => x.Id)
            .Take(Take)
            .ToDebugSql();
    }

    [Benchmark]
    public string QueryBuilder_Explain()
    {
        return _db.Query<Order>()
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .OrderByDescending(x => x.Id)
            .Take(Take)
            .Explain();
    }
}
