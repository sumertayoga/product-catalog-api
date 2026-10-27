using ProductCatalog.Application.Common.Cqrs;
using ProductCatalog.Application.Common.Models;

namespace ProductCatalog.Application.Products.Queries;

public sealed record SearchProductsQuery(ProductSearchRequest Request);

internal sealed class SearchProductsQueryHandler(
    IProductRepository repository) : IQueryHandler<SearchProductsQuery, PagedResult<ProductResponse>>
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

        var (items, totalCount) = await repository.SearchAsync(criteria, cancellationToken);

        return new PagedResult<ProductResponse>(
            items.Select(p => p.ToResponse()).ToList(),
            request.Page,
            request.PageSize,
            totalCount);
    }
}
