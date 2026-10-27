using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Exceptions;

namespace ProductCatalog.UnitTests.Domain;

public class ProductTests
{
    private static readonly DateTime Now = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_TrimsNameAndDefaultsDescription()
    {
        var product = Product.Create("  Laptop  ", null, 999.99m, Now);

        Assert.Equal("Laptop", product.Name);
        Assert.Equal(string.Empty, product.Description);
        Assert.Equal(999.99m, product.Price);
        Assert.Equal(Now, product.CreatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithoutName_Throws(string name)
    {
        Assert.Throws<DomainException>(() => Product.Create(name, null, 10m, Now));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithNonPositivePrice_Throws(decimal price)
    {
        Assert.Throws<DomainException>(() => Product.Create("Laptop", null, price, Now));
    }

    [Fact]
    public void Update_ChangesDetailsButKeepsCreatedAt()
    {
        var product = Product.Create("Laptop", "Old", 10m, Now);

        product.Update("Phone", "New", 20m);

        Assert.Equal("Phone", product.Name);
        Assert.Equal("New", product.Description);
        Assert.Equal(20m, product.Price);
        Assert.Equal(Now, product.CreatedAt);
    }
}
