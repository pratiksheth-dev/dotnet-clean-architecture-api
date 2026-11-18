namespace CleanArchitecture.Domain.Entities;

/// <summary>Lifecycle states for an <see cref="Order"/>.</summary>
public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Cancelled = 3
}
