namespace ProductCatalog.Application.Products;

public sealed record ProductSearchCriteria(
    string? Name,
    decimal? MinPrice,
    decimal? MaxPrice,
    int Page,
    int PageSize);
