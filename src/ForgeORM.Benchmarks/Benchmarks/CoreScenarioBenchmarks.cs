using System.Data;
using BenchmarkDotNet.Attributes;
using Dapper;
using ForgeORM.Benchmarks.Ef;
using ForgeORM.Benchmarks.Forge;
using ForgeORM.Benchmarks.Infrastructure;
using ForgeORM.Benchmarks.Models;
using ForgeORM.Benchmarks.Sql;
using ForgeORM.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeORM.Benchmarks.Benchmarks;

/// <summary>
/// Stable benchmark matrix for ForgeORM freeze validation.
/// Keep ForgeDbContext cached like Dapper's cached mapper path; do not create it inside benchmark methods.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class CoreScenarioBenchmarks
{
    private ServiceProvider _provider = default!;
    private BenchmarkSettings _settings = default!;
    private SqlConnectionFactory _connectionFactory = default!;
    private IDbContextFactory<BenchmarkDbContext> _efFactory = default!;
    private ForgeDbContext _forge = default!;

    [Params(10, 50, 100)]
    public int Take { get; set; }

    [GlobalSetup]
    public async Task Setup()
    {
        _provider = BenchmarkServices.Build();
        _settings = _provider.GetRequiredService<BenchmarkSettings>();
        _connectionFactory = _provider.GetRequiredService<SqlConnectionFactory>();
        _efFactory = _provider.GetRequiredService<IDbContextFactory<BenchmarkDbContext>>();
        _forge = ForgeDbContextFactory.Create();

        await _provider.GetRequiredService<DatabaseSeeder>().RecreateAndSeedAsync();
    }

    [Benchmark(Baseline = true)]
    public async Task<OrderDto?> Dapper_Query_By_Id()
    {
        await using var connection = _connectionFactory.Create();
        return await connection.QueryFirstOrDefaultAsync<OrderDto>(
            new CommandDefinition(BenchmarkSql.QueryById, new { Id = _settings.QueryOrderId }));
    }

    [Benchmark]
    public async Task<OrderDto?> EF_Core_Query_By_Id()
    {
        await using var db = await _efFactory.CreateDbContextAsync();
        return await db.Orders
            .AsNoTracking()
            .Where(o => o.Id == _settings.QueryOrderId)
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
            .FirstOrDefaultAsync();
    }

    [Benchmark]
    public async Task<OrderDto?> ForgeORM_Query_By_Id()
        => await _forge.QueryFirstOrDefaultAsync<OrderDto>(BenchmarkSql.QueryById, new { Id = _settings.QueryOrderId });

    [Benchmark]
    public async Task<OrderDto?> Dapper_Query_First()
    {
        await using var connection = _connectionFactory.Create();
        return await connection.QueryFirstOrDefaultAsync<OrderDto>(
            new CommandDefinition(BenchmarkSql.QueryFirst, new { CustomerId = _settings.QueryCustomerId }));
    }

    [Benchmark]
    public async Task<OrderDto?> EF_Core_Query_First()
    {
        await using var db = await _efFactory.CreateDbContextAsync();
        return await db.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == _settings.QueryCustomerId)
            .OrderByDescending(o => o.Id)
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
            .FirstOrDefaultAsync();
    }

    [Benchmark]
    public async Task<OrderDto?> ForgeORM_Query_First()
        => await _forge.QueryFirstOrDefaultAsync<OrderDto>(BenchmarkSql.QueryFirst, new { CustomerId = _settings.QueryCustomerId });

    [Benchmark]
    public async Task<IReadOnlyList<OrderDto>> Dapper_Query_List()
    {
        await using var connection = _connectionFactory.Create();
        var rows = await connection.QueryAsync<OrderDto>(
            new CommandDefinition(BenchmarkSql.QueryList, new { CustomerId = _settings.QueryCustomerId, Take }));
        return rows.AsList();
    }

    [Benchmark]
    public async Task<List<OrderDto>> EF_Core_Query_List()
    {
        await using var db = await _efFactory.CreateDbContextAsync();
        return await db.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == _settings.QueryCustomerId)
            .OrderByDescending(o => o.Id)
            .Take(Take)
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
            .ToListAsync();
    }

    [Benchmark]
    public async Task<IReadOnlyList<OrderDto>> ForgeORM_Query_List()
        => await _forge.QueryAsync<OrderDto>(BenchmarkSql.QueryList, new { CustomerId = _settings.QueryCustomerId, Take });

    [Benchmark]
    public async Task<IReadOnlyList<OrderDto>> Dapper_Search_Paged()
    {
        await using var connection = _connectionFactory.Create();
        var rows = await connection.QueryAsync<OrderDto>(
            new CommandDefinition(BenchmarkSql.SearchPaged, new { CustomerId = _settings.QueryCustomerId, Skip = 0, Take }));
        return rows.AsList();
    }

    [Benchmark]
    public async Task<List<OrderDto>> EF_Core_Search_Paged()
    {
        await using var db = await _efFactory.CreateDbContextAsync();
        return await db.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == _settings.QueryCustomerId)
            .OrderByDescending(o => o.Id)
            .Skip(0)
            .Take(Take)
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
            .ToListAsync();
    }

    [Benchmark]
    public async Task<IReadOnlyList<OrderDto>> ForgeORM_Search_Paged()
        => await _forge.QueryAsync<OrderDto>(BenchmarkSql.SearchPaged, new { CustomerId = _settings.QueryCustomerId, Skip = 0, Take });

    [Benchmark]
    public async Task<int> Dapper_Insert_Single()
    {
        await using var connection = _connectionFactory.Create();
        return await connection.ExecuteScalarAsync<int>(BenchmarkSql.InsertOrderSql, NewOrder());
    }

    [Benchmark]
    public async Task<int> EF_Core_Insert_Single()
    {
        await using var db = await _efFactory.CreateDbContextAsync();
        var order = NewOrder();
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        return order.Id;
    }

    [Benchmark]
    public async Task<int> ForgeORM_Insert_Single()
        => await _forge.ExecuteScalarAsync<int>(BenchmarkSql.InsertOrderSql, NewOrder());

    [Benchmark]
    public async Task<int> Dapper_Insert_Bulk()
    {
        await using var connection = _connectionFactory.Create();
        var rows = NewOrders(Take);
        return await connection.ExecuteAsync(BenchmarkSql.InsertOrderSql, rows);
    }

    [Benchmark]
    public async Task<int> EF_Core_Insert_Bulk()
    {
        await using var db = await _efFactory.CreateDbContextAsync();
        var rows = NewOrders(Take);
        db.Orders.AddRange(rows);
        return await db.SaveChangesAsync();
    }

    [Benchmark]
    public async Task<int> ForgeORM_Insert_Bulk()
    {
        var rows = NewOrders(Take);
        var inserted = 0;
        for (var i = 0; i < rows.Length; i++)
            inserted += await _forge.ExecuteScalarAsync<int>(BenchmarkSql.InsertOrderSql, rows[i]) > 0 ? 1 : 0;
        return inserted;
    }

    [Benchmark]
    public async Task<int> Dapper_Update()
    {
        var id = await InsertOneDapperAsync();
        await using var connection = _connectionFactory.Create();
        return await connection.ExecuteAsync(BenchmarkSql.UpdateOrderStatusSql, new { Id = id, Status = "Paid" });
    }

    [Benchmark]
    public async Task<int> EF_Core_Update()
    {
        var id = await InsertOneDapperAsync();
        await using var db = await _efFactory.CreateDbContextAsync();
        var order = await db.Orders.FirstAsync(o => o.Id == id);
        order.Status = "Paid";
        return await db.SaveChangesAsync();
    }

    [Benchmark]
    public async Task<int> ForgeORM_Update()
    {
        var id = await InsertOneDapperAsync();
        return await _forge.ExecuteAsync(BenchmarkSql.UpdateOrderStatusSql, new { Id = id, Status = "Paid" });
    }

    [Benchmark]
    public async Task<int> Dapper_Delete()
    {
        var id = await InsertOneDapperAsync();
        await using var connection = _connectionFactory.Create();
        return await connection.ExecuteAsync(BenchmarkSql.DeleteOrderSql, new { Id = id });
    }

    [Benchmark]
    public async Task<int> EF_Core_Delete()
    {
        var id = await InsertOneDapperAsync();
        await using var db = await _efFactory.CreateDbContextAsync();
        var order = await db.Orders.FirstAsync(o => o.Id == id);
        db.Orders.Remove(order);
        return await db.SaveChangesAsync();
    }

    [Benchmark]
    public async Task<int> ForgeORM_Delete()
    {
        var id = await InsertOneDapperAsync();
        return await _forge.ExecuteAsync(BenchmarkSql.DeleteOrderSql, new { Id = id });
    }

    [Benchmark]
    public async Task<int> ForgeORM_Graph_Insert()
    {
        var order = BenchmarkDataFactory.NewOrderGraph(_settings.QueryCustomerId, 3);
        return await _forge.InsertGraphAsync<Order, OrderItem, int>(
            order,
            x => x.Items,
            x => x.Id,
            x => x.OrderId);
    }

    [Benchmark]
    public async Task<int> ForgeORM_Graph_Update()
    {
        var order = BenchmarkDataFactory.NewOrderGraph(_settings.QueryCustomerId, 3);
        var id = await _forge.InsertGraphAsync<Order, OrderItem, int>(
            order,
            x => x.Items,
            x => x.Id,
            x => x.OrderId);

        order.Id = id;
        order.GrandTotal += 10;
        return await _forge.UpdateGraphAsync(order, options => options.IncludeChildren = true);
    }

    [Benchmark]
    public async Task<int> Dapper_Split_Query()
    {
        await using var connection = _connectionFactory.Create();
        using var grid = await connection.QueryMultipleAsync(
            BenchmarkSql.SplitQueryParent + "\n" + BenchmarkSql.SplitQueryChildren,
            new { Id = _settings.QueryOrderId });
        var order = await grid.ReadAsync<Order>();
        var items = (await grid.ReadAsync<OrderItem>()).AsList();
        return (order is null ? 0 : 1) + items.Count;
    }

    [Benchmark]
    public async Task<int> EF_Core_Split_Query()
    {
        await using var db = await _efFactory.CreateDbContextAsync();
        var order = await db.Orders
            .AsNoTracking()
            .AsSplitQuery()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == _settings.QueryOrderId);
        return (order is null ? 0 : 1) + (order?.Items.Count ?? 0);
    }

    [Benchmark]
    public async Task<int> ForgeORM_Split_Query()
    {
        var order = await _forge.QueryFirstOrDefaultAsync<Order>(BenchmarkSql.SplitQueryParent, new { Id = _settings.QueryOrderId });
        var items = await _forge.QueryAsync<OrderItem>(BenchmarkSql.SplitQueryChildren, new { Id = _settings.QueryOrderId });
        return (order is null ? 0 : 1) + items.Count;
    }

    [Benchmark]
    public async Task<OrderRecordDto?> Dapper_Record_DTO()
    {
        await using var connection = _connectionFactory.Create();
        return await connection.QueryFirstOrDefaultAsync<OrderRecordDto>(
            new CommandDefinition(BenchmarkSql.RecordDto, new { Id = _settings.QueryOrderId }));
    }

    [Benchmark]
    public async Task<OrderRecordDto?> ForgeORM_Record_DTO()
        => await _forge.QueryFirstOrDefaultAsync<OrderRecordDto>(BenchmarkSql.RecordDto, new { Id = _settings.QueryOrderId });

    [Benchmark]
    public async Task<OrderEnumDto?> Dapper_Enum_Mapping()
    {
        await using var connection = _connectionFactory.Create();
        return await connection.QueryFirstOrDefaultAsync<OrderEnumDto>(
            new CommandDefinition(BenchmarkSql.EnumMapping, new { Status = "Paid" }));
    }

    [Benchmark]
    public async Task<OrderEnumDto?> ForgeORM_Enum_Mapping()
        => await _forge.QueryFirstOrDefaultAsync<OrderEnumDto>(BenchmarkSql.EnumMapping, new { Status = "Paid" });

    [Benchmark]
    public async Task<int> Dapper_Streaming()
    {
        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync();
        await using var command = new SqlCommand(BenchmarkSql.QueryList, connection);
        command.Parameters.AddWithValue("@CustomerId", _settings.QueryCustomerId);
        command.Parameters.AddWithValue("@Take", Take);
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);
        var count = 0;
        while (await reader.ReadAsync())
            count++;
        return count;
    }

    [Benchmark]
    public async Task<int> EF_Core_Async_Streaming()
    {
        await using var db = await _efFactory.CreateDbContextAsync();
        var count = 0;
        await foreach (var _ in db.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == _settings.QueryCustomerId)
            .OrderByDescending(o => o.Id)
            .Take(Take)
            .AsAsyncEnumerable())
        {
            count++;
        }
        return count;
    }

    [Benchmark]
    public async Task<int> ForgeORM_Async_Streaming()
    {
        var rows = await _forge.QueryAsync<OrderDto>(BenchmarkSql.QueryList, new { CustomerId = _settings.QueryCustomerId, Take });
        return rows.Count;
    }

    [Benchmark]
    public async Task<OrderDto?> Dapper_Stored_Procedure()
    {
        await using var connection = _connectionFactory.Create();
        return await connection.QueryFirstOrDefaultAsync<OrderDto>(
            BenchmarkSql.StoredProcedureName,
            new { Id = _settings.QueryOrderId },
            commandType: CommandType.StoredProcedure);
    }

    [Benchmark]
    public async Task<OrderDto?> ForgeORM_Stored_Procedure()
        => await _forge.QueryFirstOrDefaultAsync<OrderDto>(
            "EXEC dbo.GetOrderByIdForBenchmark @Id",
            new { Id = _settings.QueryOrderId });

    [GlobalCleanup]
    public void Cleanup() => _provider.Dispose();

    private async Task<int> InsertOneDapperAsync()
    {
        await using var connection = _connectionFactory.Create();
        return await connection.ExecuteScalarAsync<int>(BenchmarkSql.InsertOrderSql, NewOrder());
    }

    private Order NewOrder()
    {
        var order = BenchmarkDataFactory.NewOrder(_settings.QueryCustomerId);
        order.CustomerId = _settings.QueryCustomerId;
        return order;
    }

    private Order[] NewOrders(int count)
    {
        var rows = new Order[count];
        for (var i = 0; i < rows.Length; i++)
            rows[i] = NewOrder();
        return rows;
    }
}
