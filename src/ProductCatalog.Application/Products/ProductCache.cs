using ProductCatalog.Application.Common.Abstractions;
using ProductCatalog.Application.Common.Models;

namespace ProductCatalog.Application.Products;

// Search results are keyed by a version token; any write bumps the version so all cached searches go stale at once.
internal sealed class ProductCache(ICacheService cache)
{
    private const string SearchVersionKey = "products:search-version";
    private static readonly TimeSpan ProductTtl = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan SearchTtl = TimeSpan.FromMinutes(1);

    public Task<ProductResponse?> GetProductAsync(
        int id,
        CancellationToken cancellationToken) =>
        cache.GetAsync<ProductResponse>(ProductKey(id), cancellationToken);

    public Task SetProductAsync(
        ProductResponse product,
        CancellationToken cancellationToken) =>
        cache.SetAsync(ProductKey(product.Id), product, ProductTtl, cancellationToken);

    public async Task<PagedResult<ProductResponse>?> GetSearchAsync(
        ProductSearchCriteria criteria,
        CancellationToken cancellationToken) =>
        await cache.GetAsync<PagedResult<ProductResponse>>(
            await SearchKeyAsync(criteria, cancellationToken),
            cancellationToken);

    public async Task SetSearchAsync(
        ProductSearchCriteria criteria,
        PagedResult<ProductResponse> result,
        CancellationToken cancellationToken) =>
        await cache.SetAsync(
            await SearchKeyAsync(criteria, cancellationToken),
            result,
            SearchTtl,
            cancellationToken);

    public async Task InvalidateAsync(
        int? productId,
        CancellationToken cancellationToken)
    {
        if (productId is { } id)
            await cache.RemoveAsync(ProductKey(id), cancellationToken);

        await cache.SetAsync(SearchVersionKey, Guid.NewGuid().ToString("N"), null, cancellationToken);
    }

    private static string ProductKey(int id) => $"products:{id}";

    private async Task<string> SearchKeyAsync(
        ProductSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var version = await cache.GetAsync<string>(SearchVersionKey, cancellationToken) ?? "0";
        return FormattableString.Invariant(
            $"products:search:{version}:{criteria.Name?.ToLowerInvariant()}:{criteria.MinPrice}:{criteria.MaxPrice}:{criteria.Page}:{criteria.PageSize}");
    }
}
