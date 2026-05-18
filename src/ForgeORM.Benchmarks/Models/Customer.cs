using ForgeORM.Abstractions;

namespace ForgeORM.Benchmarks.Models;

[ForgeTable("Customers")]
public sealed class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? City { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
