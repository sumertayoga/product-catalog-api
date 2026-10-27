using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Products;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<(IReadOnlyList<Product> Items, int TotalCount)> SearchAsync(
        ProductSearchCriteria criteria,
        CancellationToken cancellationToken);

    void Add(Product product);

    void Remove(Product product);
}
