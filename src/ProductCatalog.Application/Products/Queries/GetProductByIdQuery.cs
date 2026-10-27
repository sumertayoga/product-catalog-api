using ProductCatalog.Application.Common.Cqrs;
using ProductCatalog.Application.Common.Exceptions;

namespace ProductCatalog.Application.Products.Queries;

public sealed record GetProductByIdQuery(int Id);

internal sealed class GetProductByIdQueryHandler(
    IProductRepository repository,
    ProductCache cache) : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<ProductResponse> HandleAsync(
        GetProductByIdQuery query,
        CancellationToken cancellationToken)
    {
        var cached = await cache.GetProductAsync(query.Id, cancellationToken);
        if (cached is not null)
            return cached;

        var product = await repository.GetByIdAsync(query.Id, cancellationToken)
                      ?? throw new NotFoundException($"Product {query.Id} was not found.");

        var response = product.ToResponse();
        await cache.SetProductAsync(response, cancellationToken);
        return response;
    }
}
