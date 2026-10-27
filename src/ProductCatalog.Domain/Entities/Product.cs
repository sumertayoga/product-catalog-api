using ProductCatalog.Domain.Exceptions;

namespace ProductCatalog.Domain.Entities;

public sealed class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Product()
    {
    }

    public static Product Create(
        string name,
        string? description,
        decimal price,
        DateTime createdAtUtc)
    {
        var product = new Product
        {
            CreatedAt = createdAtUtc
        };
        product.SetDetails(name,
            description,
            price);
        return product;
    }

    public void Update(string name,
        string? description,
        decimal price) =>
        SetDetails(name,
            description,
            price);

    private void SetDetails(string name,
        string? description,
        decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required.");
        if (price <= 0)
            throw new DomainException("Price must be greater than zero.");

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
        Price = price;
    }
}
