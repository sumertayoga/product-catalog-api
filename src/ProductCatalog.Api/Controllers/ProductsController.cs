using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.Common.Cqrs;
using ProductCatalog.Application.Common.Models;
using ProductCatalog.Application.Products;
using ProductCatalog.Application.Products.Commands;
using ProductCatalog.Application.Products.Queries;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResult<ProductResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search(
        [FromQuery] ProductSearchRequest request,
        [FromServices] IQueryHandler<SearchProductsQuery, PagedResult<ProductResponse>> handler,
        CancellationToken cancellationToken) =>
        Ok(await handler.HandleAsync(new SearchProductsQuery(request), cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        int id,
        [FromServices] IQueryHandler<GetProductByIdQuery, ProductResponse> handler,
        CancellationToken cancellationToken) =>
        Ok(await handler.HandleAsync(new GetProductByIdQuery(id), cancellationToken));

    [HttpPost]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        ProductRequest request,
        [FromServices] ICommandHandler<CreateProductCommand, ProductResponse> handler,
        CancellationToken cancellationToken)
    {
        var product = await handler.HandleAsync(new CreateProductCommand(request), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        ProductRequest request,
        [FromServices] ICommandHandler<UpdateProductCommand> handler,
        CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new UpdateProductCommand(id, request), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        [FromServices] ICommandHandler<DeleteProductCommand> handler,
        CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new DeleteProductCommand(id), cancellationToken);
        return NoContent();
    }
}
