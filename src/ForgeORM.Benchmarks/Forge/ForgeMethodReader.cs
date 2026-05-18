using BenchmarkDotNet.Attributes;
using ForgeORM.Benchmarks.Models;
using ForgeORM.Core;

namespace ForgeORM.Benchmarks.Forge;

[MemoryDiagnoser]
public class ForgeExpressionMethodBenchmarks
{
    private ForgeDbContext _db = default!;

    [Params(1, 2, 3)]
    public int CustomerId { get; set; }

    [Params(10, 50, 100)]
    public int Take { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _db = ForgeDbContextFactory.Create();
    }

    [Benchmark]
    public async Task<IReadOnlyList<Order>> Expression_Where_ToListAsync()
    {
        return await _db.Set<Order>()
            .Where(x => x.CustomerId == CustomerId)
            .ToListAsync(includeChildren: false);
    }

    [Benchmark]
    public async Task<Order?> Expression_FirstOrDefaultAsync()
    {
        return await _db.Set<Order>()
            .Where(x => x.CustomerId == CustomerId)
            .FirstOrDefaultAsync(includeChildren:false);
    }

    [Benchmark]
    public async Task<int> Expression_CountAsync()
    {
        return await _db.Set<Order>()
            .Where(x => x.CustomerId == CustomerId)
            .CountAsync();
    }

    [Benchmark]
    public async Task<bool> Expression_AnyAsync()
    {
        return await _db.Set<Order>()
            .Where(x => x.CustomerId == CustomerId)
            .AnyAsync();
    }

    [Benchmark]
    public async Task<IReadOnlyList<Order>> Expression_Where_OrderByDescending_Skip_Take_ToListAsync()
    {
        return  await _db.Set<Order>()
            .Where(x => x.CustomerId == CustomerId)
            .OrderByDescending(x => x.Id)
            .Skip(0)
            .Take(Take)
            .ToListAsync(includeChildren: false);
    }

    [Benchmark]
    public async Task<IReadOnlyList<Order>> Sql_Where_ToListAsync()
    {
        return _db.Query<Order>(
                """
                SELECT
                    Id,
                    OrderNo,
                    CustomerId,
                    OrderDate,
                    Status,
                    SubTotal,
                    Tax,
                    GrandTotal,
                    CreatedAt
                FROM dbo.Orders
                WHERE CustomerId = @CustomerId
                """,
                new { CustomerId })
            .ToList();
    }

    [Benchmark]
    public async Task<Order?> Sql_FirstOrDefaultAsync()
    {
        return _db.Query<Order>(
                """
                SELECT TOP 1
                    Id,
                    OrderNo,
                    CustomerId,
                    OrderDate,
                    Status,
                    SubTotal,
                    Tax,
                    GrandTotal,
                    CreatedAt
                FROM dbo.Orders
                WHERE CustomerId = @CustomerId
                ORDER BY Id DESC
                """,
                new { CustomerId })
            .FirstOrDefault();
    }

    [Benchmark]
    public async Task<int> Sql_CountAsync()
    {
        return _db.Query<Order>(
                """
                SELECT
                    Id,
                    OrderNo,
                    CustomerId,
                    OrderDate,
                    Status,
                    SubTotal,
                    Tax,
                    GrandTotal,
                    CreatedAt
                FROM dbo.Orders
                WHERE CustomerId = @CustomerId
                """,
                new { CustomerId })
            .Count();
    }

    [Benchmark]
    public async Task<bool> Sql_AnyAsync()
    {
        return _db.Query<Order>(
                """
                SELECT TOP 1
                    Id,
                    OrderNo,
                    CustomerId,
                    OrderDate,
                    Status,
                    SubTotal,
                    Tax,
                    GrandTotal,
                    CreatedAt
                FROM dbo.Orders
                WHERE CustomerId = @CustomerId
                """,
                new { CustomerId })
            .Any();
    }

    [Benchmark]
    public async Task<IReadOnlyList<Order>> Sql_Where_OrderByDescending_Skip_Take_ToListAsync()
    {
        return _db.Query<Order>(
                """
                SELECT
                    Id,
                    OrderNo,
                    CustomerId,
                    OrderDate,
                    Status,
                    SubTotal,
                    Tax,
                    GrandTotal,
                    CreatedAt
                FROM dbo.Orders
                WHERE CustomerId = @CustomerId
                ORDER BY Id DESC
                OFFSET 0 ROWS FETCH NEXT @Take ROWS ONLY
                """,
                new { CustomerId, Take })
            .ToList();
    }
}