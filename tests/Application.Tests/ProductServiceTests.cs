using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Application.Tests.Fakes;
using Xunit;

namespace CleanArchitecture.Application.Tests;

public class ProductServiceTests
{
    private readonly InMemoryProductRepository _repository = new();
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _service = new ProductService(_repository);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_ReturnsCreatedProduct()
    {
        var request = new CreateProductRequest("Widget", "A useful widget", 9.99m, 10);

        var result = await _service.CreateAsync(request);

        Assert.Equal("Widget", result.Name);
        Assert.Equal(9.99m, result.Price);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task CreateAsync_WithNegativePrice_ThrowsValidationException()
    {
        var request = new CreateProductRequest("Widget", null, -1m, 10);

        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(request));
    }

    [Fact]
    public async Task GetByIdAsync_WithUnknownId_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(999));
    }

    [Fact]
    public async Task DeleteAsync_RemovesProduct()
    {
        var created = await _service.CreateAsync(
            new CreateProductRequest("Widget", null, 5m, 1));

        await _service.DeleteAsync(created.Id);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(created.Id));
    }
}
