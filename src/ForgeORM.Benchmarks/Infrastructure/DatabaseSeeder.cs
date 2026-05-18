using Dapper;
using ForgeORM.Benchmarks.Sql;

namespace ForgeORM.Benchmarks.Infrastructure;

public sealed class DatabaseSeeder
{
    private readonly SqlConnectionFactory _connectionFactory;
    private readonly BenchmarkSettings _settings;

    public DatabaseSeeder(SqlConnectionFactory connectionFactory, BenchmarkSettings settings)
    {
        _connectionFactory = connectionFactory;
        _settings = settings;
    }

    public async Task RecreateAndSeedAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.Create();
        await connection.OpenAsync(cancellationToken);
        await connection.ExecuteAsync(BenchmarkSql.CreateSchema);

        await connection.ExecuteAsync("""
INSERT INTO dbo.Categories (Name, Description)
VALUES
('Electronics', 'Electronic products'),
('Books', 'Books and learning material'),
('Fashion', 'Clothing and accessories'),
('Home', 'Home and kitchen products'),
('Sports', 'Sports products');
""");

        for (var productIndex = 1; productIndex <= 100; productIndex++)
        {
            await connection.ExecuteAsync("""
INSERT INTO dbo.Products(CategoryId, Sku, Name, UnitPrice, StockQuantity, IsActive, CreatedAt)
VALUES (@CategoryId, @Sku, @Name, @UnitPrice, @StockQuantity, 1, SYSUTCDATETIME());
""", new
            {
                CategoryId = ((productIndex - 1) % 5) + 1,
                Sku = $"SKU-{productIndex:00000}",
                Name = $"Product {productIndex:00000}",
                UnitPrice = 50 + productIndex,
                StockQuantity = 1000 - productIndex
            });
        }

        for (var customerIndex = 1; customerIndex <= _settings.SeedCustomers; customerIndex++)
        {
            var customerId = await connection.ExecuteScalarAsync<int>("""
INSERT INTO dbo.Customers(FullName, Email, Phone, City, IsActive, CreatedAt)
VALUES (@FullName, @Email, @Phone, @City, 1, SYSUTCDATETIME());
SELECT CAST(SCOPE_IDENTITY() AS INT);
""", new
            {
                FullName = $"Customer {customerIndex:00000}",
                Email = $"customer{customerIndex:00000}@example.com",
                Phone = $"0300{customerIndex:0000000}",
                City = customerIndex % 2 == 0 ? "Karachi" : "Lahore"
            });

            for (var orderIndex = 1; orderIndex <= _settings.SeedOrdersPerCustomer; orderIndex++)
            {
                var status = orderIndex % 4 == 0 ? "Paid" : orderIndex % 4 == 1 ? "Processing" : orderIndex % 4 == 2 ? "Pending" : "Cancelled";
                var subTotal = 100 + orderIndex;
                var tax = subTotal * 0.15m;
                var grandTotal = subTotal + tax;

                var orderId = await connection.ExecuteScalarAsync<int>("""
INSERT INTO dbo.Orders(OrderNo, CustomerId, OrderDate, Status, SubTotal, Tax, GrandTotal, CreatedAt)
VALUES (@OrderNo, @CustomerId, DATEADD(DAY, -@DaysBack, SYSUTCDATETIME()), @Status, @SubTotal, @Tax, @GrandTotal, SYSUTCDATETIME());
SELECT CAST(SCOPE_IDENTITY() AS INT);
""", new
                {
                    OrderNo = $"ORD-{customerIndex:00000}-{orderIndex:00000}",
                    CustomerId = customerId,
                    Status = status,
                    SubTotal = subTotal,
                    Tax = tax,
                    GrandTotal = grandTotal,
                    DaysBack = orderIndex
                });

                for (var itemIndex = 1; itemIndex <= 3; itemIndex++)
                {
                    var productId = ((orderIndex + itemIndex - 1) % 100) + 1;
                    var quantity = itemIndex + 1;
                    var price = 50 + productId;
                    await connection.ExecuteAsync("""
INSERT INTO dbo.OrderItems(OrderId, ProductId, Quantity, UnitPrice, LineTotal)
VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @LineTotal);
""", new
                    {
                        OrderId = orderId,
                        ProductId = productId,
                        Quantity = quantity,
                        UnitPrice = price,
                        LineTotal = quantity * price
                    });
                }

                if (status == "Paid")
                {
                    await connection.ExecuteAsync("""
INSERT INTO dbo.Payments(OrderId, PaymentDate, Amount, Method, Status)
VALUES (@OrderId, SYSUTCDATETIME(), @Amount, @Method, 'Completed');
""", new
                    {
                        OrderId = orderId,
                        Amount = grandTotal,
                        Method = orderIndex % 2 == 0 ? "Card" : "Cash"
                    });
                }
            }
        }
    }
}
