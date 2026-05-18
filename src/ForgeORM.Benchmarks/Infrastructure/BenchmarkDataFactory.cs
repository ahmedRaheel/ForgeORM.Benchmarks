using ForgeORM.Benchmarks.Models;

namespace ForgeORM.Benchmarks.Infrastructure;

public static class BenchmarkDataFactory
{
    private static int _orderNoSeed = Environment.TickCount;

    public static Order NewOrder(int customerId = 1)
    {
        var seed = Interlocked.Increment(ref _orderNoSeed);
        var total = 1000 + seed % 5000;

        return new Order
        {
            OrderNo = $"Forge-{Guid.NewGuid():N}",
            CustomerId = 1,
            SubTotal = 500,
            Tax = 75,
            GrandTotal = 575,
            Status = "Processing",
            OrderDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Order NewOrderGraph(int customerId = 1, int itemCount = 3)
    {
        var order = NewOrder(customerId);
        for (var i = 1; i <= itemCount; i++)
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

    public static IReadOnlyList<Order> NewOrders(int count, int customerId = 1)
    {
        var rows = new List<Order>(count);
        for (var i = 0; i < count; i++)
            rows.Add(NewOrder(customerId));
        return rows;
    }

    //public static BulkOrder NewBulkOrder(int customerId = 1)
    //{
    //    var order = NewOrder(customerId);
    //    return new BulkOrder
    //    {
    //        CustomerId = order.CustomerId,
    //        OrderNo = order.OrderNo,
    //        Status = order.Status,
    //        GrandTotal = order.GrandTotal,
    //        TotalAmount = order.TotalAmount,
    //        OrderDate = order.OrderDate,
    //        CreatedAt = order.CreatedAt
    //    };
    //}

    //public static IReadOnlyList<BulkOrder> NewBulkOrders(int count, int customerId = 1)
    //{
    //    var rows = new List<BulkOrder>(count);
    //    for (var i = 0; i < count; i++)
    //        rows.Add(NewBulkOrder(customerId));
    //    return rows;
    //}

    public static Product NewProduct(int categoryId = 1)
    {
        var seed = Interlocked.Increment(ref _orderNoSeed);
        return new Product
        {
            CategoryId = categoryId,
            Sku = $"BENCH-SKU-{seed}",
            Name = $"Benchmark Product {seed}",
            UnitPrice = 25 + seed % 500,
            StockQuantity = 100,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }
}
