using ProductCatalog.Application.Common.Abstractions;
using ProductCatalog.Application.Common.Cqrs;
using ProductCatalog.Application.Common.Exceptions;

namespace ProductCatalog.Application.Products.Commands;

public sealed record UpdateProductCommand(
    int Id,
    ProductRequest Request);

internal sealed class UpdateProductCommandHandler(
    IProductRepository repository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateProductCommand>
{
    public async Task HandleAsync(
        UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(command.Id, cancellationToken)
                      ?? throw new NotFoundException($"Product {command.Id} was not found.");

        var request = command.Request;
        product.Update(
            request.Name,
            request.Description,
            request.Price);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
