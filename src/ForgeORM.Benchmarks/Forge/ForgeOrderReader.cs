using Azure;
using ForgeORM.Abstractions;
using ForgeORM.Benchmarks.Models;
using ForgeORM.Benchmarks.Sql;
using ForgeORM.Core;
using ForgeORM.Core.Performance;


namespace ForgeORM.Benchmarks.Forge;

/// <summary>
/// Adapter for ForgeORM queries. Keep benchmark classes stable and only edit this file when ForgeORM API changes.
/// </summary>
public sealed class ForgeOrderReader
{
    private readonly string _connectionString;
    private readonly ForgeDbContext db;
    public ForgeOrderReader(string connectionString)
    {
        _connectionString = connectionString;
        db = ForgeDbContextFactory.Create();
    }

    public  async Task<Order?> GetByIdAsync(int id, CancellationToken ct = default)
    {      

        return await  db.QueryFirstOrDefaultAsync<Order>(BenchmarkSql.QueryById, new { Id = id });
        
    }

    public  async Task<IReadOnlyList<OrderDto>> SearchPagedAsync(int customerId, int skip, int take, CancellationToken ct = default)
    {   
        return await  db.QueryAsync<OrderDto>(BenchmarkSql.SearchPaged, new { CustomerId = customerId, Skip = skip, Take = take }, cancellationToken: ct);
    }
}
