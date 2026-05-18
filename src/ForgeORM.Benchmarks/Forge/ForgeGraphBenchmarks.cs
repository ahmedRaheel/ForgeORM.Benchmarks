using BenchmarkDotNet.Attributes;
using ForgeORM.Benchmarks.Infrastructure;
using ForgeORM.Benchmarks.Models;
using ForgeORM.Core;
using ForgeORM.Core.Graph;

namespace ForgeORM.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks graph insert, update and delete methods. These tests are intentionally smaller than bulk tests
/// because graph methods execute multiple statements and relationship binding work.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class ForgeGraphBenchmarks
{
    private ForgeDbContext _db = default!;
    private BenchmarkSettings _settings = default!;

    [Params(1, 3, 5)]
    public int ChildCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _settings = new BenchmarkSettings();
        _db = ForgeBenchmarkDbFactory.Create(_settings.ConnectionString);
    }

    [Benchmark(Baseline = true)]
    public Task<int> InsertGraphAsync_Order_With_Items()
    {
        var order = BenchmarkDataFactory.NewOrderGraph(_settings.QueryCustomerId, ChildCount);

        return _db.InsertGraphAsync<Order, OrderItem, int>(
            order,
            x => x.Items,
            x => x.Id,
            x => x.OrderId);
    }

    [Benchmark]
    public async Task<int> UpdateGraphAsync_Order_With_Items()
    {
        var order = BenchmarkDataFactory.NewOrderGraph(_settings.QueryCustomerId, ChildCount);
        var id = await _db.InsertGraphAsync<Order, OrderItem, int>(
            order,
            x => x.Items,
            x => x.Id,
            x => x.OrderId);

        var existing = await _db.GetByIdAsync<Order>(id) ?? order;
        existing.Id = id;
        existing.GrandTotal += 10;
        

        return await _db.UpdateGraphAsync(existing, options =>
        {
            options.IncludeChildren = true;
            options.ChildSyncMode = ForgeChildSyncMode.InsertUpdate;
        });
    }

    [Benchmark]
    public async Task<int> DeleteGraphAsync_Order_With_Items()
    {
        var order = BenchmarkDataFactory.NewOrderGraph(_settings.QueryCustomerId, ChildCount);
        var id = await _db.InsertGraphAsync<Order, OrderItem, int>(
            order,
            x => x.Items,
            x => x.Id,
            x => x.OrderId);

        return await _db.DeleteGraphAsync<Order>(id, options =>
        {
            options.IncludeChildren = true;
            options.DeleteMode = ForgeDeleteMode.HardDelete;
        });
    }
}
