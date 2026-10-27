using System.ComponentModel.DataAnnotations;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Products;

public sealed record ProductRequest(
    [Required, StringLength(100, MinimumLength = 1)] string Name,
    [StringLength(1000)] string? Description,
    [Range(typeof(decimal), "0.01", "1000000")] decimal Price);

public sealed record ProductResponse(
    int Id,
    string Name,
    string Description,
    decimal Price,
    DateTime CreatedAt);

public sealed class ProductSearchRequest : IValidatableObject
{
    [StringLength(100)]
    public string? Name { get; init; }

    [Range(typeof(decimal), "0", "1000000")]
    public decimal? MinPrice { get; init; }

    [Range(typeof(decimal), "0", "1000000")]
    public decimal? MaxPrice { get; init; }

    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, 100)]
    public int PageSize { get; init; } = 20;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MinPrice > MaxPrice)
            yield return new ValidationResult(
                "MinPrice must be less than or equal to MaxPrice.",
                [nameof(MinPrice), nameof(MaxPrice)]);
    }
}

public static class ProductMappings
{
    public static ProductResponse ToResponse(this Product product) =>
        new(product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.CreatedAt);
}
