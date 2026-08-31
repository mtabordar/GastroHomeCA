namespace GastroHomeCA.Domain.UnitTests.Entities;

using GastroHomeCA.Domain.Entities;
using Xunit;
using Shouldly;

public class ProductTests
{
    [Fact]
    public void Create_ShouldThrow_WhenNameEmpty()
    {
        // Arrange
        var product = new Product();

        // Act
        var exception = Record.Exception(() => 
            product.Create(string.Empty, "Category", null, 10.99m));

        // Assert
        exception.ShouldBeOfType<ArgumentException>();
    }

    [Fact]
    public void Create_ShouldThrow_WhenNameWhitespace()
    {
        // Arrange
        var product = new Product();

        // Act
        var exception = Record.Exception(() => 
            product.Create("   ", "Category", null, 10.99m));

        // Assert
        exception.ShouldBeOfType<ArgumentException>();
    }

    [Fact]
    public void Create_ShouldThrow_WhenCategoryEmpty()
    {
        // Arrange
        var product = new Product();

        // Act
        var exception = Record.Exception(() => 
            product.Create("Product", string.Empty, null, 10.99m));

        // Assert
        exception.ShouldBeOfType<ArgumentException>();
    }

    [Fact]
    public void Create_ShouldThrow_WhenCategoryWhitespace()
    {
        // Arrange
        var product = new Product();

        // Act
        var exception = Record.Exception(() => 
            product.Create("Product", "   ", null, 10.99m));

        // Assert
        exception.ShouldBeOfType<ArgumentException>();
    }

    [Fact]
    public void Create_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var product = new Product();

        // Act
        product.Create("Test Product", "Electronics", "1234567", 29.99m);

        // Assert
        product.Name.ShouldBe("Test Product");
        product.Category.ShouldBe("Electronics");
        product.Barcode.ShouldBe("1234567");
        product.CurrentPrice.ShouldBe(29.99m);
    }

    [Fact]
    public void UpdatePrice_ShouldThrow_WhenPriceNegative()
    {
        // Arrange
        var product = new Product();
        product.Create("Product", "Category", null, 10.99m);

        // Act
        var exception = Record.Exception(() => 
            product.UpdatePrice(-5.99m));

        // Assert
        exception.ShouldBeOfType<ArgumentException>();
    }

    [Fact]
    public void UpdatePrice_ShouldUpdatePriceCorrectly()
    {
        // Arrange
        var product = new Product();
        product.Create("Product", "Category", null, 10.99m);

        // Act
        product.UpdatePrice(15.99m);

        // Assert
        product.CurrentPrice.ShouldBe(15.99m);
    }
}