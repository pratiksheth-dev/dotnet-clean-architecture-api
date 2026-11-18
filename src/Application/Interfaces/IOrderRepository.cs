using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Interfaces;

/// <summary>Persistence abstraction for orders, implemented in Infrastructure.</summary>
public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the order with its line items loaded, or null.</summary>
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);
}
