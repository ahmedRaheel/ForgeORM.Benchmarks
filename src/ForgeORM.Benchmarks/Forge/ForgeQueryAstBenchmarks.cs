using BenchmarkDotNet.Attributes;
using ForgeORM.Benchmarks.Forge;
using ForgeORM.Benchmarks.Infrastructure;
using ForgeORM.Benchmarks.Models;
using ForgeORM.Core;
using ForgeORM.QueryAst;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeORM.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks ForgeSQL / QueryAst rendering and execution APIs.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class ForgeQueryAstBenchmarks
{
    private ServiceProvider _provider = default!;
    private ForgeDbContext _db = default!;
    [Params(1, 2, 3)]
    public int CustomerId { get; set; }
    [Params(1, 2, 3)]
    public int OrderId { get; set; }

    [Params(10, 50, 100)]
    public int Take { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _provider = BenchmarkServices.Build();
        _db = ForgeDbContextFactory.Create();
    }

    [Benchmark(Baseline = true)]
    public string ForgeSql_Render_Select_Where_Order_Page()
    {
        var rendered = ForgeSql.Select<Order>()
            .Columns(x => x.Id, x => x.CustomerId, x => x.OrderNo, x => x.GrandTotal)
            .From("Orders")
            .Where(x => x.CustomerId == CustomerId)
            .OrderByDescending(x => x.Id)
            .Skip(0)
            .Take(Take)
            .Render(_db.Provider);

        return rendered.Sql;
    }

    //[Benchmark]
    //public Task<IReadOnlyList<Order>> ForgeSql_Execute_ToListAsync()
    //{
    //    return ForgeSql.Select<Order>()
    //        .Columns(x => x.Id, x => x.CustomerId, x => x.OrderNo, x => x.Status, x => x.GrandTotal,  x => x.CreatedAt, x => x.OrderDate)
    //        .From("Orders")
    //        .Where(x => x.CustomerId == CustomerId)
    //        .OrderByDescending(x => x.Id)
    //        .Skip(0)
    //        .Take(Take)
    //        .ToListAsync<Order, Order>(_db);
    //}

    [Benchmark]
    public async Task<Order?> ForgeSql_Execute_FirstOrDefaultAsync()
    {
        return await ForgeSql.Select<Order>()
            .Columns(x => x.Id, x => x.CustomerId, x => x.OrderNo, x => x.Status, x => x.GrandTotal, x => x.CreatedAt, x => x.OrderDate)
            .From("Orders")
            .Where(x => x.Id == CustomerId)
            .FirstOrDefaultAsync<Order,Order>(_db);
    }

    [Benchmark]
    public async Task<int> ForgeSql_Execute_CountAsync()
    {
        return await ForgeSql.Select<Order>()
            .From("Orders")
            .Where(x => x.CustomerId == CustomerId)
            .CountAsync(_db);
    }

    //[Benchmark]
    //public Task<bool> ForgeSql_Execute_AnyAsync()
    //{
    //    return ForgeSql.Select<Order>()
    //        .From("Orders")
    //        .Where(x => x.CustomerId == CustomerId)
    //        .AnyAsync(_db);
    //}

    //[Benchmark]
    //public string QueryAst_Render_GroupBy_Having()
    //{
    //    var rendered = ForgeSql.Select<Order>()
    //        .ColumnsSql("Status", "COUNT(1) AS TotalOrders", "SUM(GrandTotal) AS TotalSales")
    //        .From("Orders")
    //        .GroupBy("Status")
    //        .HavingCount(">=", 5)
    //        .OrderBySql("TotalOrders DESC")
    //        .Render(_db.Provider);

    //    return rendered.Sql;
    //}

    //[Benchmark]
    //public Task<IReadOnlyList<StatusAggregate>> QueryAst_Execute_GroupBy_Having()
    //{
    //    var rendered = ForgeSql.Select<Order>()
    //        .ColumnsSql("Status", "COUNT(1) AS TotalOrders", "SUM(GrandTotal) AS TotalSales")
    //        .From("Orders")
    //        .GroupBy("Status")
    //        .HavingCount(">=", 5)
    //        .OrderBySql("TotalOrders DESC")
    //        .Render(_db.Provider);

    //    return _db.QueryAsync<StatusAggregate>(rendered.Sql, rendered.Parameters);
    //}

    //[Benchmark]
    //public string QueryAst_Render_Join_Projection()
    //{
    //    var rendered = ForgeSql.Select<Order>()
    //        .ColumnsSql("o.Id", "o.OrderNo", "o.CustomerId", "o.Status", "o.GrandTotal")
    //        .From("Orders o")
    //        .InnerJoin("Customers c", "c.Id = o.CustomerId")
    //        .WhereSql("o.GrandTotal >= @MinTotal", new { MinTotal = 100m })
    //        .OrderBySql("o.Id DESC")
    //        .Take(Take)
    //        .Render(_db.Provider);

    //    return rendered.Sql;
    //}

    //[Benchmark]
    //public string QueryAst_Render_Update()
    //{
    //    var rendered = ForgeSql.Select<Order>()
    //        .From("Orders")
    //        .Where(x => x.Id == OrderId)
    //        .RenderUpdate(_db.Provider, new { TotalAmount = 999m });

    //    return rendered.Sql;
    //}

    //[Benchmark]
    //public string QueryAst_Render_Delete()
    //{
    //    var rendered = ForgeSql.Select<Order>()
    //        .From("Orders")
    //        .Where(x => x.Id == OrderId)
    //        .RenderDelete(_db.Provider);

    //    return rendered.Sql;
    //}
    [GlobalCleanup]
    public void Cleanup() => _provider.Dispose();
}
