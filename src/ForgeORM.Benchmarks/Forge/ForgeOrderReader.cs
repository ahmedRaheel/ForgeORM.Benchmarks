using Azure;
using ForgeORM.Benchmarks.Models;
using ForgeORM.Benchmarks.Sql;


namespace ForgeORM.Benchmarks.Forge;

/// <summary>
/// Adapter for ForgeORM queries. Keep benchmark classes stable and only edit this file when ForgeORM API changes.
/// </summary>
public sealed class ForgeOrderReader
{
    private readonly string _connectionString;

    public ForgeOrderReader(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<OrderDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var db = ForgeDbContextFactory.Create();

        return await db.QueryFirstOrDefaultAsync<OrderDto>(BenchmarkSql.QueryById, new { Id = id }, cancellationToken: ct);
    }

    public async Task<IReadOnlyList<OrderDto>> SearchPagedAsync(int customerId, int skip, int take, CancellationToken ct = default)
    {
        var db = ForgeDbContextFactory.Create();

        var products = await db.Search<Product>()
    .FullText("wireless keyboard")
    .Fuzzy()
    .Top(20)
    .ToListAsync(ct);

        var matches = await db.Vector<Product>()
            .SearchAsync(queryEmbedding, topK: 10, metric: VectorMetric.Cosine, ct);

        var path = await db.Graph()
            .From<Customer>(customerId)
            .Traverse("PLACED_ORDER")
            .ShortestPathTo<Product>(productId)
            .ToListAsync(ct);
        return await db.QueryAsync<OrderDto>(BenchmarkSql.SearchPaged, new { CustomerId = customerId, Skip = skip, Take = take }, cancellationToken: ct);
    }
}
