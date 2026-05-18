namespace ForgeORM.Benchmarks.Models;

public sealed class OrderSearchCriteria
{
    public int CustomerId { get; set; }
    public string Status { get; set; } = "Paid";
    public decimal MinTotal { get; set; } = 100;
    public int Skip { get; set; }
    public int Take { get; set; } = 50;
}
