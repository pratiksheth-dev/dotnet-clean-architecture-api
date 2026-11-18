namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// A single line on an <see cref="Order"/>. Unit price is captured at order
/// time so later catalog price changes do not alter historical orders.
/// </summary>
public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public Order? Order { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
}
