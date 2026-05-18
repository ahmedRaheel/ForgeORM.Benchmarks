using BenchmarkDotNet.Attributes;
using ForgeORM.Abstractions;
using ForgeORM.Benchmarks.Infrastructure;
using ForgeORM.Benchmarks.Models;
using ForgeORM.Core;

namespace ForgeORM.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks raw SQL, scalar, first/single, execute and built-in paging APIs.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class ForgeRawSqlBenchmarks
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
    public Task<IReadOnlyList<Order>> QueryAsync_ToList()
    {
        return _db.QueryAsync<Order>(
            """
            SELECT Id, CustomerId, OrderNo, Status, GrandTotal, TotalAmount, CreatedAt, OrderDate
            FROM Orders
            WHERE CustomerId = @CustomerId
            ORDER BY Id DESC
            OFFSET 0 ROWS FETCH NEXT @Take ROWS ONLY
            """,
            new { CustomerId = _settings.QueryCustomerId, Take });
    }

    [Benchmark]
    public Task<Order?> QueryFirstOrDefaultAsync()
    {
        return _db.QueryFirstOrDefaultAsync<Order>(
            """
            SELECT TOP 1 Id, CustomerId, OrderNo, Status, GrandTotal, TotalAmount, CreatedAt, OrderDate
            FROM Orders
            WHERE CustomerId = @CustomerId
            ORDER BY Id DESC
            """,
            new { CustomerId = _settings.QueryCustomerId });
    }

    [Benchmark]
    public Task<Order?> QuerySingleOrDefaultAsync()
    {
        return _db.QuerySingleOrDefaultAsync<Order>(
            """
            SELECT TOP 1 Id, CustomerId, OrderNo, Status, GrandTotal, TotalAmount, CreatedAt, OrderDate
            FROM Orders
            WHERE Id = @Id
            """,
            new { Id = _settings.QueryOrderId });
    }

    [Benchmark]
    public async Task<int> ExecuteScalar_CountAsync()
    {
        return await _db.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Orders WHERE CustomerId = @CustomerId",
            new { CustomerId = _settings.QueryCustomerId });
    }

    [Benchmark]
    public Task<int> ExecuteAsync_NoOpUpdate()
    {
        return _db.ExecuteAsync(
            "UPDATE Orders SET TotalAmount = TotalAmount WHERE Id = @Id",
            new { Id = _settings.QueryOrderId });
    }

    [Benchmark]
    public Task<ForgePagedResult<Order>> PageAsync_RawSql()
    {
        return _db.PageAsync<Order>(new ForgePageRequest
        {
            Sql = "SELECT Id, CustomerId, OrderNo, Status, GrandTotal, TotalAmount, CreatedAt, OrderDate FROM Orders WHERE CustomerId = @CustomerId",
            Parameters = new { CustomerId = _settings.QueryCustomerId },
            Page = 1,
            PageSize = Take,
            OrderBy = "Id DESC"
        });
    }

    [Benchmark]
    public Task<IReadOnlyList<Order>> Sql_Composable_Query_ToListAsync()
    {
        return _db.Sql<Order>(
                "SELECT Id, CustomerId, OrderNo, Status, GrandTotal, TotalAmount, CreatedAt, OrderDate FROM Orders",
                null)
            .WhereSql("CustomerId = @CustomerId", new { CustomerId = _settings.QueryCustomerId })
            .OrderBySql("Id DESC")
            .Take(Take)
            .ToListAsync();
    }
}
