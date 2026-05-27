namespace ForgeORM.Benchmarks.Models;

public sealed class OrderEnumDto
{
    public int Id { get; set; }
    public string OrderNo { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
}
