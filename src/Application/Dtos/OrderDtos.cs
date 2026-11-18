namespace CleanArchitecture.Application.Dtos;

public record OrderItemDto(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);

public record OrderDto(
    int Id,
    string CustomerEmail,
    string Status,
    DateTime CreatedAtUtc,
    IReadOnlyList<OrderItemDto> Items,
    decimal Total);

public record CreateOrderItemRequest(int ProductId, int Quantity);

public record CreateOrderRequest(
    string CustomerEmail,
    IReadOnlyList<CreateOrderItemRequest> Items);
