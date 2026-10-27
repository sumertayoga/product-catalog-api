using ProductCatalog.Application.Common.Abstractions;
using ProductCatalog.Application.Common.Cqrs;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Products.Commands;

public sealed record CreateProductCommand(ProductRequest Request);

internal sealed class CreateProductCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork,
    ProductCache cache,
    TimeProvider timeProvider) : ICommandHandler<CreateProductCommand, ProductResponse>
{
    public async Task<ProductResponse> HandleAsync(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            timeProvider.GetUtcNow().UtcDateTime);

        repository.Add(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cache.InvalidateAsync(null, cancellationToken);

        return product.ToResponse();
    }
}
