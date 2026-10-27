using ProductCatalog.Application.Common.Cqrs;
using ProductCatalog.Application.Common.Models;

namespace ProductCatalog.Application.Products.Queries;

public sealed record SearchProductsQuery(ProductSearchRequest Request);

internal sealed class SearchProductsQueryHandler(
    IProductRepository repository,
    ProductCache cache) : IQueryHandler<SearchProductsQuery, PagedResult<ProductResponse>>
{
    public async Task<PagedResult<ProductResponse>> HandleAsync(
        SearchProductsQuery query,
        CancellationToken cancellationToken)
    {
        var request = query.Request;
        var criteria = new ProductSearchCriteria(
            string.IsNullOrWhiteSpace(request.Name) ? null : request.Name.Trim(),
            request.MinPrice,
            request.MaxPrice,
            request.Page,
            request.PageSize);

        var cached = await cache.GetSearchAsync(criteria, cancellationToken);
        if (cached is not null)
            return cached;

        var (items, totalCount) = await repository.SearchAsync(criteria, cancellationToken);

        var result = new PagedResult<ProductResponse>(
            items.Select(p => p.ToResponse()).ToList(),
            request.Page,
            request.PageSize,
            totalCount);

        await cache.SetSearchAsync(criteria, result, cancellationToken);
        return result;
    }
}
