using ProductCatalog.Application.Common.Cqrs;
using ProductCatalog.Application.Common.Exceptions;

namespace ProductCatalog.Application.Products.Queries;

public sealed record GetProductByIdQuery(int Id);

internal sealed class GetProductByIdQueryHandler(
    IProductRepository repository) : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<ProductResponse> HandleAsync(
        GetProductByIdQuery query,
        CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(query.Id, cancellationToken)
                      ?? throw new NotFoundException($"Product {query.Id} was not found.");

        return product.ToResponse();
    }
}
