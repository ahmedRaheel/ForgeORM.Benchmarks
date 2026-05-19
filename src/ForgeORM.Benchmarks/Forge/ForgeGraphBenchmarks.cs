using BenchmarkDotNet.Attributes;
using ForgeORM.Benchmarks.Forge;
using ForgeORM.Benchmarks.Infrastructure;
using ForgeORM.Benchmarks.Models;
using ForgeORM.Core;
using ForgeORM.Core.Graph;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeORM.Benchmarks.Benchmarks;

/// <summary>
/// Benchmarks graph insert, update and delete methods. These tests are intentionally smaller than bulk tests
/// because graph methods execute multiple statements and relationship binding work.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class ForgeGraphBenchmarks
{
    private ServiceProvider _provider = default!;
    private ForgeDbContext _db = default!;
    [Params(1, 2, 3)]
    public int CustomerId { get; set; }
    [Params(1, 3, 5)]
    public int ChildCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _provider = BenchmarkServices.Build();
        _db = ForgeDbContextFactory.Create();
    }     

    [Benchmark(Baseline = true)]
    public Task<int> InsertGraphAsync_Order_With_Items()
    {
        var order = NewOrderGraph();

        return _db.InsertGraphAsync<Order, OrderItem, int>(
            order,
            x => x.Items,
            x => x.Id,
            x => x.OrderId);
    }

    [Benchmark]
    public async Task<int> UpdateGraphAsync_Order_With_Items()
    {
        var order = NewOrderGraph();
        var id = await _db.InsertGraphAsync<Order, OrderItem, int>(
            order,
            x => x.Items,
            x => x.CustomerId,
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
        var order = NewOrderGraph();
        var id = await _db.InsertGraphAsync<Order, OrderItem, int>(
            order,
            x => x.Items,
           x => x.CustomerId,
            x => x.OrderId);

        return await _db.DeleteGraphAsync<Order>(id, options =>
        {
            options.IncludeChildren = true;
            options.DeleteMode = ForgeDeleteMode.HardDelete;
        });
    }

    public  Order NewOrderGraph()
    {
        var order = new Order
        {
            OrderNo = $"Forge-{Guid.NewGuid():N}",
            CustomerId = CustomerId,
            SubTotal = 500,
            Tax = 75,
            GrandTotal = 575,
            Status = "Processing",
            OrderDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        }; 
        for (var i = 1; i <= ChildCount ; i++)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = i,
                Quantity = i,
                UnitPrice = 100 + i,
                LineTotal = (100 + i) * i
            });
        }

        return order;
    }
}
