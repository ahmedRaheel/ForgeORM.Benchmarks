namespace ForgeORM.Benchmarks.Models;

public sealed record OrderRecordDto(
    int Id,
    string OrderNo,
    int CustomerId,
    string CustomerName,
    decimal GrandTotal,
    string Status,
    DateTime OrderDate);
