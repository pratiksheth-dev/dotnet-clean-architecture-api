using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orders;

    public OrdersController(OrderService orders)
    {
        _orders = orders;
    }

    /// <summary>Lists all orders, most recent first.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _orders.GetAllAsync(cancellationToken));
    }

    /// <summary>Gets a single order (with line items) by id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await _orders.GetByIdAsync(id, cancellationToken));
    }

    /// <summary>
    /// Places an order. Validates stock, decrements inventory, and snapshots
    /// unit prices. Requires authentication.
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(
        CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var created = await _orders.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
