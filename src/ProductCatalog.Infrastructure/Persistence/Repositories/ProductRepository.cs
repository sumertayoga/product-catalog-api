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

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> SearchAsync(
        ProductSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Products.AsNoTracking();

        if (criteria.Name is not null)
        {
            var pattern = $"%{EscapeLikePattern(criteria.Name)}%";
            query = query.Where(p => EF.Functions.ILike(p.Name, pattern, @"\"));
        }

        if (criteria.MinPrice is { } minPrice)
            query = query.Where(p => p.Price >= minPrice);

        if (criteria.MaxPrice is { } maxPrice)
            query = query.Where(p => p.Price <= maxPrice);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(p => p.Id)
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Product product) => dbContext.Products.Add(product);

    public void Remove(Product product) => dbContext.Products.Remove(product);

    private static string EscapeLikePattern(string value) =>
        value.Replace(@"\", @"\\")
            .Replace("%", @"\%")
            .Replace("_", @"\_");
}
