//using BenchmarkDotNet.Attributes;
//using ForgeORM.Benchmarks.Infrastructure;
//using ForgeORM.Benchmarks.Models;
//using ForgeORM.Core;

//namespace ForgeORM.Benchmarks.Benchmarks;

///// <summary>
///// Benchmarks bulk insert, bulk update and bulk delete APIs.
///// </summary>
//[MemoryDiagnoser]
//[SimpleJob(warmupCount: 3, iterationCount: 10)]
//public class ForgeBulkBenchmarks
//{
//    private ForgeDbContext _db = default!;
//    private BenchmarkSettings _settings = default!;

//    [Params(10, 100, 500)]
//    public int Rows { get; set; }

//    [GlobalSetup]
//    public void Setup()
//    {
//        _settings = new BenchmarkSettings();
//        _db = ForgeBenchmarkDbFactory.Create(_settings.ConnectionString);
//    }

//    [Benchmark(Baseline = true)]
//    public Task BulkInsertAsync_Orders()
//    {
//        var rows = BenchmarkDataFactory.NewBulkOrders(Rows, _settings.QueryCustomerId)
//            .Select(ToInsertRow)
//            .ToArray();

//        return _db.BulkInsertAsync("Orders", rows);
//    }

//    [Benchmark]
//    public async Task BulkUpdateAsync_Orders()
//    {
//        var rows = BenchmarkDataFactory.NewBulkOrders(Rows, _settings.QueryCustomerId).ToList();
//        await _db.BulkInsertAsync("Orders", rows.Select(ToInsertRow).ToArray());

//        var inserted = await _db.QueryAsync<BulkOrder>(
//            """
//            SELECT TOP (@Rows) Id, CustomerId, OrderNo, Status, GrandTotal, TotalAmount, CreatedAt, OrderDate
//            FROM Orders
//            WHERE CustomerId = @CustomerId
//            ORDER BY Id DESC
//            """,
//            new { Rows, CustomerId = _settings.QueryCustomerId });

//        foreach (var row in inserted)
//        {
//            row.GrandTotal += 1;
//            row.TotalAmount = row.GrandTotal;
//        }

//        await _db.BulkUpdateAsync(inserted.ToList());
//    }

//    [Benchmark]
//    public async Task BulkDeleteAsync_Orders()
//    {
//        var rows = BenchmarkDataFactory.NewBulkOrders(Rows, _settings.QueryCustomerId);
//        await _db.BulkInsertAsync("Orders", rows.Select(ToInsertRow).ToArray());

//        var inserted = await _db.QueryAsync<BulkOrder>(
//            """
//            SELECT TOP (@Rows) Id, CustomerId, OrderNo, Status, GrandTotal, TotalAmount, CreatedAt, OrderDate
//            FROM Orders
//            WHERE CustomerId = @CustomerId
//            ORDER BY Id DESC
//            """,
//            new { Rows, CustomerId = _settings.QueryCustomerId });

//        await _db.BulkDeleteAsync<BulkOrder>(inserted.Select(x => x.Id).ToArray());
//    }

//    private static object ToInsertRow(BulkOrder row)
//    {
//        return new
//        {
//            row.CustomerId,
//            row.OrderNo,
//            Status = row.Status.ToString(),
//            row.GrandTotal,
//            row.TotalAmount,
//            row.CreatedAt,
//            row.OrderDate
//        };
//    }
//}
