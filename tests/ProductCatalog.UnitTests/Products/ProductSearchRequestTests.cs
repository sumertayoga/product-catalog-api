using System.ComponentModel.DataAnnotations;
using ProductCatalog.Application.Products;

namespace ProductCatalog.UnitTests.Products;

public class ProductSearchRequestTests
{
    [Fact]
    public void MinPriceGreaterThanMaxPrice_IsInvalid()
    {
        var request = new ProductSearchRequest { MinPrice = 100, MaxPrice = 10 };

        Assert.NotEmpty(Validate(request));
    }

    [Fact]
    public void ValidRange_IsValid()
    {
        var request = new ProductSearchRequest { MinPrice = 10, MaxPrice = 100 };

        Assert.Empty(Validate(request));
    }

    private static List<ValidationResult> Validate(object request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);
        return results;
    }
}
