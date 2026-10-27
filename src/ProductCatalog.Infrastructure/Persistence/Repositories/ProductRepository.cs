using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Products;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken) =>
        dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<(IReadOnlyList<Product> Items, int TotalCount)> SearchAsync(
        ProductSearchCriteria criteria,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public void Add(Product product) => dbContext.Products.Add(product);

    public void Remove(Product product) => dbContext.Products.Remove(product);
}
