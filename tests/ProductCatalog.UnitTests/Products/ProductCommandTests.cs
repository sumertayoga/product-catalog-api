using NSubstitute;
using ProductCatalog.Application.Common.Abstractions;
using ProductCatalog.Application.Common.Exceptions;
using ProductCatalog.Application.Products;
using ProductCatalog.Application.Products.Commands;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.UnitTests.Products;

public class ProductCommandTests
{
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICacheService _cache = Substitute.For<ICacheService>();

    [Fact]
    public async Task Create_AddsProductAndSaves()
    {
        var handler = new CreateProductCommandHandler(_repository, _unitOfWork, new ProductCache(_cache), TimeProvider.System);

        var result = await handler.HandleAsync(
            new CreateProductCommand(new ProductRequest("Laptop", "Fast", 999m)),
            CancellationToken.None);

        Assert.Equal("Laptop", result.Name);
        _repository.Received(1).Add(Arg.Is<Product>(p => p.Name == "Laptop"));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_UnknownProduct_ThrowsNotFound()
    {
        _repository.GetByIdAsync(42, Arg.Any<CancellationToken>()).Returns((Product?)null);
        var handler = new UpdateProductCommandHandler(_repository, _unitOfWork, new ProductCache(_cache));

        await Assert.ThrowsAsync<NotFoundException>(() => handler.HandleAsync(
            new UpdateProductCommand(42, new ProductRequest("Laptop", null, 10m)),
            CancellationToken.None));

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_ClearsCachedProduct()
    {
        var product = Product.Create("Laptop", null, 10m, DateTime.UtcNow);
        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(product);
        var handler = new UpdateProductCommandHandler(_repository, _unitOfWork, new ProductCache(_cache));

        await handler.HandleAsync(
            new UpdateProductCommand(1, new ProductRequest("Phone", null, 20m)),
            CancellationToken.None);

        Assert.Equal("Phone", product.Name);
        await _cache.Received(1).RemoveAsync("products:1", Arg.Any<CancellationToken>());
    }
}
