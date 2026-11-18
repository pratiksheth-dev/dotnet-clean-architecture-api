using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Services;

/// <summary>
/// Order use cases. Placing an order validates the request, checks stock,
/// decrements inventory, and snapshots unit prices onto the order lines.
/// </summary>
public class OrderService
{
    private readonly IOrderRepository _orders;
    private readonly IProductRepository _products;

    public OrderService(IOrderRepository orders, IProductRepository products)
    {
        _orders = orders;
        _products = products;
    }

    public async Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orders.GetAllAsync(cancellationToken);
        return orders.Select(ToDto).ToList();
    }

    public async Task<OrderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _orders.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), id);

        return ToDto(order);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request);

        var order = new Order
        {
            CustomerEmail = request.CustomerEmail.Trim(),
            Status = OrderStatus.Confirmed
        };

        foreach (var line in request.Items)
        {
            var product = await _products.GetByIdAsync(line.ProductId, cancellationToken)
                ?? throw new NotFoundException(nameof(Product), line.ProductId);

            if (product.StockQuantity < line.Quantity)
            {
                throw new ValidationException(
                    $"Insufficient stock for product '{product.Name}': " +
                    $"requested {line.Quantity}, available {product.StockQuantity}.");
            }

            product.StockQuantity -= line.Quantity;
            await _products.UpdateAsync(product, cancellationToken);

            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = line.Quantity,
                UnitPrice = product.Price
            });
        }

        var created = await _orders.AddAsync(order, cancellationToken);
        return ToDto(created);
    }

    private static void Validate(CreateOrderRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.CustomerEmail) || !request.CustomerEmail.Contains('@'))
            errors.Add("A valid customer email is required.");

        if (request.Items is null || request.Items.Count == 0)
            errors.Add("An order must contain at least one item.");

        if (request.Items is not null && request.Items.Any(i => i.Quantity <= 0))
            errors.Add("Item quantities must be greater than zero.");

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }

    internal static OrderDto ToDto(Order order) => new(
        order.Id,
        order.CustomerEmail,
        order.Status.ToString(),
        order.CreatedAtUtc,
        order.Items.Select(i => new OrderItemDto(
            i.ProductId,
            i.Product?.Name ?? string.Empty,
            i.Quantity,
            i.UnitPrice,
            i.Quantity * i.UnitPrice)).ToList(),
        order.Total);
}
