using ProductCatalog.Application.Common.Abstractions;
using ProductCatalog.Application.Common.Cqrs;
using ProductCatalog.Application.Common.Exceptions;

namespace ProductCatalog.Application.Products.Commands;

public sealed record DeleteProductCommand(int Id);

internal sealed class DeleteProductCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork,
    ProductCache cache) : ICommandHandler<DeleteProductCommand>
{
    public async Task HandleAsync(
        DeleteProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(command.Id, cancellationToken)
                      ?? throw new NotFoundException($"Product {command.Id} was not found.");

        repository.Remove(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cache.InvalidateAsync(command.Id, cancellationToken);
    }
}
