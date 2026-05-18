using BenchmarkDotNet.Attributes;
using ForgeORM.Benchmarks.Infrastructure;
using ForgeORM.Benchmarks.Models;
using ForgeORM.Core;
using ForgeORM.QueryAst;

namespace ForgeORM.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks ForgeSQL / QueryAst rendering and execution APIs.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class ForgeQueryAstBenchmarks
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
    public string ForgeSql_Render_Select_Where_Order_Page()
    {
        var rendered = ForgeSql.Select<Order>()
            .Columns(x => x.Id, x => x.CustomerId, x => x.OrderNo, x => x.GrandTotal)
            .From("Orders")
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .OrderByDescending(x => x.Id)
            .Skip(0)
            .Take(Take)
            .Render(_db.Provider);

        return rendered.Sql;
    }

    [Benchmark]
    public Task<IReadOnlyList<Order>> ForgeSql_Execute_ToListAsync()
    {
        return ForgeSql.Select<Order>()
            .Columns(x => x.Id, x => x.CustomerId, x => x.OrderNo, x => x.Status, x => x.GrandTotal,  x => x.CreatedAt, x => x.OrderDate)
            .From("Orders")
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .OrderByDescending(x => x.Id)
            .Skip(0)
            .Take(Take)
            .ToListAsync<Order, Order>(_db);
    }

    [Benchmark]
    public Task<Order?> ForgeSql_Execute_FirstOrDefaultAsync()
    {
        return ForgeSql.Select<Order>()
            .Columns(x => x.Id, x => x.CustomerId, x => x.OrderNo, x => x.Status, x => x.GrandTotal, x => x.CreatedAt, x => x.OrderDate)
            .From("Orders")
            .Where(x => x.Id == _settings.QueryOrderId)
            .FirstOrDefaultAsync<Order, Order>(_db);
    }

    [Benchmark]
    public Task<int> ForgeSql_Execute_CountAsync()
    {
        return ForgeSql.Select<Order>()
            .From("Orders")
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .CountAsync(_db);
    }

    [Benchmark]
    public Task<bool> ForgeSql_Execute_AnyAsync()
    {
        return ForgeSql.Select<Order>()
            .From("Orders")
            .Where(x => x.CustomerId == _settings.QueryCustomerId)
            .AnyAsync(_db);
    }

    [Benchmark]
    public string QueryAst_Render_GroupBy_Having()
    {
        var rendered = ForgeSql.Select<Order>()
            .ColumnsSql("Status", "COUNT(1) AS TotalOrders", "SUM(GrandTotal) AS TotalSales")
            .From("Orders")
            .GroupBy("Status")
            .HavingCount(">=", 5)
            .OrderBySql("TotalOrders DESC")
            .Render(_db.Provider);

        return rendered.Sql;
    }

    [Benchmark]
    public Task<IReadOnlyList<StatusAggregate>> QueryAst_Execute_GroupBy_Having()
    {
        var rendered = ForgeSql.Select<Order>()
            .ColumnsSql("Status", "COUNT(1) AS TotalOrders", "SUM(GrandTotal) AS TotalSales")
            .From("Orders")
            .GroupBy("Status")
            .HavingCount(">=", 5)
            .OrderBySql("TotalOrders DESC")
            .Render(_db.Provider);

        return _db.QueryAsync<StatusAggregate>(rendered.Sql, rendered.Parameters);
    }

    [Benchmark]
    public string QueryAst_Render_Join_Projection()
    {
        var rendered = ForgeSql.Select<Order>()
            .ColumnsSql("o.Id", "o.OrderNo", "o.CustomerId", "o.Status", "o.GrandTotal")
            .From("Orders o")
            .InnerJoin("Customers c", "c.Id = o.CustomerId")
            .WhereSql("o.GrandTotal >= @MinTotal", new { MinTotal = 100m })
            .OrderBySql("o.Id DESC")
            .Take(Take)
            .Render(_db.Provider);

        return rendered.Sql;
    }

    [Benchmark]
    public string QueryAst_Render_Update()
    {
        var rendered = ForgeSql.Select<Order>()
            .From("Orders")
            .Where(x => x.Id == _settings.QueryOrderId)
            .RenderUpdate(_db.Provider, new { TotalAmount = 999m });

        return rendered.Sql;
    }

    [Benchmark]
    public string QueryAst_Render_Delete()
    {
        var rendered = ForgeSql.Select<Order>()
            .From("Orders")
            .Where(x => x.Id == _settings.QueryOrderId)
            .RenderDelete(_db.Provider);

        return rendered.Sql;
    }

}
