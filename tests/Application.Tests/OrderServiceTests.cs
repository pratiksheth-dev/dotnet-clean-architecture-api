using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Application.Tests.Fakes;
using Xunit;

namespace CleanArchitecture.Application.Tests;

public class OrderServiceTests
{
    private readonly InMemoryProductRepository _products = new();
    private readonly InMemoryOrderRepository _orders = new();
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _service = new OrderService(_orders, _products);
    }

    private async Task<int> SeedProductAsync(decimal price, int stock)
    {
        var productService = new ProductService(_products);
        var created = await productService.CreateAsync(
            new CreateProductRequest("Gadget", null, price, stock));
        return created.Id;
    }

    [Fact]
    public async Task CreateAsync_WithValidItems_ComputesTotalAndDecrementsStock()
    {
        var productId = await SeedProductAsync(20m, 5);
        var request = new CreateOrderRequest(
            "customer@example.com",
            new[] { new CreateOrderItemRequest(productId, 2) });

        var order = await _service.CreateAsync(request);

        Assert.Equal(40m, order.Total);
        Assert.Equal("Confirmed", order.Status);

        var product = await _products.GetByIdAsync(productId);
        Assert.NotNull(product);
        Assert.Equal(3, product.StockQuantity);
    }

    [Fact]
    public async Task CreateAsync_WithInsufficientStock_ThrowsValidationException()
    {
        var productId = await SeedProductAsync(20m, 1);
        var request = new CreateOrderRequest(
            "customer@example.com",
            new[] { new CreateOrderItemRequest(productId, 5) });

        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WithUnknownProduct_ThrowsNotFoundException()
    {
        var request = new CreateOrderRequest(
            "customer@example.com",
            new[] { new CreateOrderItemRequest(999, 1) });

        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WithNoItems_ThrowsValidationException()
    {
        var request = new CreateOrderRequest(
            "customer@example.com",
            Array.Empty<CreateOrderItemRequest>());

        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(request));
    }
}
