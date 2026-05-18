using ForgeORM.Benchmarks.Models;
using Microsoft.EntityFrameworkCore;

namespace ForgeORM.Benchmarks.Ef;

public sealed class EfOrderReader
{
    private readonly IDbContextFactory<BenchmarkDbContext> _factory;

    public EfOrderReader(IDbContextFactory<BenchmarkDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<OrderDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        return await db.Orders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNo = o.OrderNo,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer!.FullName,
                GrandTotal = o.GrandTotal,
                Status = o.Status,
                OrderDate = o.OrderDate
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<OrderDto>> SearchPagedAsync(int customerId, int skip, int take, CancellationToken ct = default)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        return await db.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.Id)
            .Skip(skip)
            .Take(take)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNo = o.OrderNo,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer!.FullName,
                GrandTotal = o.GrandTotal,
                Status = o.Status,
                OrderDate = o.OrderDate
            })
            .ToListAsync(ct);
    }
}
