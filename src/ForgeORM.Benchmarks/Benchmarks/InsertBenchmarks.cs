using BenchmarkDotNet.Attributes;
using Dapper;
using ForgeORM.Benchmarks.Ef;
using ForgeORM.Benchmarks.Forge;
using ForgeORM.Benchmarks.Infrastructure;
using ForgeORM.Benchmarks.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeORM.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class InsertBenchmarks
{
    private ServiceProvider _provider = default!;
    private SqlConnectionFactory _connectionFactory = default!;
    private IDbContextFactory<BenchmarkDbContext> _efFactory = default!;

    [GlobalSetup]
    public void Setup()
    {
        _provider = BenchmarkServices.Build();
        _connectionFactory = _provider.GetRequiredService<SqlConnectionFactory>();
        _efFactory = _provider.GetRequiredService<IDbContextFactory<BenchmarkDbContext>>();
    }

    [Benchmark(Baseline = true)]
    public async Task<int> Dapper_Insert_Order()
    {
        await using var connection = _connectionFactory.Create();
        return await connection.ExecuteScalarAsync<int>("""
INSERT INTO dbo.Orders(OrderNo, CustomerId, OrderDate, Status, SubTotal, Tax, GrandTotal, CreatedAt)
VALUES (@OrderNo, @CustomerId, SYSUTCDATETIME(), @Status, @SubTotal, @Tax, @GrandTotal, SYSUTCDATETIME());
SELECT CAST(SCOPE_IDENTITY() AS INT);
""", NewOrder());
    }

    [Benchmark]
    public async Task<int> EF_Core_Insert_Order()
    {
        await using var db = await _efFactory.CreateDbContextAsync();
        var order = new Order
        {
            OrderNo = $"EF-{Guid.NewGuid():N}",
            CustomerId = 1,
            SubTotal = 500,
            Tax = 75,
            GrandTotal = 575,
            Status = "Processing",
            OrderDate = DateTime.UtcNow
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        return order.Id;
    }

    [Benchmark]
    public async Task<int> ForgeORM_Insert_Order()
    {
        // Wire this to your real ForgeORM insert API:
        var db = ForgeDbContextFactory.Create();
      
        var result= await db.ExecuteScalarAsync<int>("""
INSERT INTO dbo.Orders(OrderNo, CustomerId, OrderDate, Status, SubTotal, Tax, GrandTotal, CreatedAt)
VALUES (@OrderNo, @CustomerId, SYSUTCDATETIME(), @Status, @SubTotal, @Tax, @GrandTotal, SYSUTCDATETIME());
SELECT CAST(SCOPE_IDENTITY() AS INT);
""", NewOrder());
        return result;
       
    }
    
    [GlobalCleanup]
    public void Cleanup() => _provider.Dispose();

    private static Order NewOrder() => new Order
    {
        OrderNo = $"Forge-{Guid.NewGuid():N}",
        CustomerId = 1,
        SubTotal = 500,
        Tax = 75,
        GrandTotal = 575,
        Status = "Processing",
        OrderDate = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow
    };
}
