namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// A catalog product. Pure domain entity — no persistence or framework concerns.
/// </summary>
public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>Order lines that reference this product.</summary>
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
