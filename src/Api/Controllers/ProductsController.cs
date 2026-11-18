using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _products;

    public ProductsController(ProductService products)
    {
        _products = products;
    }

    /// <summary>Lists all products in the catalog.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _products.GetAllAsync(cancellationToken));
    }

    /// <summary>Gets a single product by id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await _products.GetByIdAsync(id, cancellationToken));
    }

    /// <summary>Creates a product. Requires authentication.</summary>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(
        CreateProductRequest request, CancellationToken cancellationToken)
    {
        var created = await _products.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Updates a product. Requires authentication.</summary>
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> Update(
        int id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _products.UpdateAsync(id, request, cancellationToken));
    }

    /// <summary>Deletes a product. Requires authentication.</summary>
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _products.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
