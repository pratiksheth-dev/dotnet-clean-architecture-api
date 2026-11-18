namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// A customer order made up of one or more <see cref="OrderItem"/> lines.
/// </summary>
public class Order
{
    public int Id { get; set; }

    public string CustomerEmail { get; set; } = string.Empty;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    /// <summary>Order total, derived from its line items.</summary>
    public decimal Total => Items.Sum(i => i.Quantity * i.UnitPrice);
}
