using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Services;

/// <summary>
/// Product catalog use cases: validation and orchestration live here,
/// persistence is delegated to <see cref="IProductRepository"/>.
/// </summary>
public class ProductService
{
    private readonly IProductRepository _products;

    public ProductService(IProductRepository products)
    {
        _products = products;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _products.GetAllAsync(cancellationToken);
        return products.Select(ToDto).ToList();
    }

    public async Task<ProductDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _products.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), id);

        return ToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Name, request.Price, request.StockQuantity);

        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };

        var created = await _products.AddAsync(product, cancellationToken);
        return ToDto(created);
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request.Name, request.Price, request.StockQuantity);

        var product = await _products.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), id);

        product.Name = request.Name.Trim();
        product.Description = request.Description;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;

        await _products.UpdateAsync(product, cancellationToken);
        return ToDto(product);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _products.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), id);

        await _products.DeleteAsync(product, cancellationToken);
    }

    private static void Validate(string name, decimal price, int stockQuantity)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add("Product name is required.");

        if (price < 0)
            errors.Add("Price cannot be negative.");

        if (stockQuantity < 0)
            errors.Add("Stock quantity cannot be negative.");

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }

    internal static ProductDto ToDto(Product product) => new(
        product.Id,
        product.Name,
        product.Description,
        product.Price,
        product.StockQuantity,
        product.CreatedAtUtc);
}
