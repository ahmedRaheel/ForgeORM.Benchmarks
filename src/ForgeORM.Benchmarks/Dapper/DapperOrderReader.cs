using Dapper;
using ForgeORM.Benchmarks.Infrastructure;
using ForgeORM.Benchmarks.Models;
using ForgeORM.Benchmarks.Sql;

namespace ForgeORM.Benchmarks.Dapper;

public sealed class DapperOrderReader
{
    private readonly SqlConnectionFactory _connectionFactory;

    public DapperOrderReader(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<OrderDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var connection = _connectionFactory.Create();
        return await connection.QueryFirstOrDefaultAsync<OrderDto>(
            new CommandDefinition(BenchmarkSql.QueryById, new { Id = id }, cancellationToken: ct));
    }

    public async Task<IReadOnlyList<OrderDto>> SearchPagedAsync(int customerId, int skip, int take, CancellationToken ct = default)
    {
        await using var connection = _connectionFactory.Create();
        var rows = await connection.QueryAsync<OrderDto>(
            new CommandDefinition(BenchmarkSql.SearchPaged, new { CustomerId = customerId, Skip = skip, Take = take }, cancellationToken: ct));

        return rows.AsList();
    }
}
