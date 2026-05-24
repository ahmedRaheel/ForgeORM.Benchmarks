using BenchmarkDotNet.Attributes;
using ForgeORM.Benchmarks.Infrastructure;
using ForgeORM.Benchmarks.Models;
using ForgeORM.Core;

namespace ForgeORM.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks repository-style CRUD methods: GetById, GetByCode, GetByIds, Insert, Update and Delete.
/// Insert/update/delete benchmarks create their own rows so repeated benchmark iterations are safe.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class ForgeCrudBenchmarks
{
    private ForgeDbContext _db = default!;
    private BenchmarkSettings _settings = default!;
    private int _rowForUpdate;
    private int _rowForDelete;

    [GlobalSetup]
    public async Task Setup()
    {
        _settings = new BenchmarkSettings();
        _db = ForgeBenchmarkDbFactory.Create(_settings.ConnectionString);
        _rowForUpdate = await InsertAndReturnIdAsync();
        _rowForDelete = await InsertAndReturnIdAsync();
    }

    [Benchmark(Baseline = true)]
    public async Task<Order?> GetByIdAsync()
    {
        return await _db.GetByIdAsync<Order>(_settings.QueryOrderId);
    }

    [Benchmark]
    public async Task<Product?> GetByCodeAsync()
    {
        return await _db.GetByCodeAsync<Product>("SKU-001");
    }

    [Benchmark]
    public async Task<IReadOnlyList<Order>> GetByIdsAsync()
    {
        return await _db.GetByIdsAsync<Order>([1, 2, 3, 4, 5]);
    }

    [Benchmark]
    public async Task<int> InsertAsync_Order()
    {
        return  await _db.InsertAsync(BenchmarkDataFactory.NewOrder(_settings.QueryCustomerId));
    }

    [Benchmark]
    public async Task<int> UpdateAsync_Order()
    {
        var order = await _db.GetByIdAsync<Order>(_rowForUpdate)
            ?? BenchmarkDataFactory.NewOrder(_settings.QueryCustomerId);

        order.GrandTotal += 1;
        
        return await _db.UpdateAsync(order);
    }

    [Benchmark]
    public async Task<int> DeleteAsync_Order()
    {
        var id = Interlocked.Exchange(ref _rowForDelete, await InsertAndReturnIdAsync());
        return await _db.DeleteAsync<Order>(id);
    }

    private async Task<int> InsertAndReturnIdAsync()
    {
        var order = BenchmarkDataFactory.NewOrder(_settings.QueryCustomerId);
        await _db.InsertAsync(order);
        var inserted = await _db.QueryFirstOrDefaultAsync<Order>(
            """
            SELECT TOP 1 Id, CustomerId, OrderNo, Status, GrandTotal, TotalAmount, CreatedAt, OrderDate
            FROM Orders
            WHERE OrderNo = @OrderNo
            ORDER BY Id DESC
            """,
            new { order.OrderNo });

        return inserted?.Id ?? 0;
    }
}
