namespace ForgeORM.Benchmarks.Models;

public sealed class StatusAggregate
{
    public string Status { get; set; } = string.Empty;
    public int TotalOrders { get; set; }
    public decimal TotalAmount { get; set; }
}
