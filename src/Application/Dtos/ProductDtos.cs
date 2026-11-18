namespace CleanArchitecture.Application.Dtos;

public record ProductDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    DateTime CreatedAtUtc);

public record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity);

public record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity);
