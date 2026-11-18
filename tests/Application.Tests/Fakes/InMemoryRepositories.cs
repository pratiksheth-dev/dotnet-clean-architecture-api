using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Tests.Fakes;

/// <summary>
/// In-memory fakes of the repository abstractions so Application services can
/// be unit-tested without a database.
/// </summary>
public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();
    private int _nextId = 1;

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Product>>(_products.ToList());
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
    }

    public Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        product.Id = _nextId++;
        _products.Add(product);
        return Task.FromResult(product);
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        _products.Remove(product);
        return Task.CompletedTask;
    }
}

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = new();
    private int _nextId = 1;

    public Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Order>>(_orders.ToList());
    }

    public Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_orders.FirstOrDefault(o => o.Id == id));
    }

    public Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        order.Id = _nextId++;
        _orders.Add(order);
        return Task.FromResult(order);
    }
}
