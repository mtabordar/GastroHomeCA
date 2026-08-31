namespace GastroHomeCA.Application.UnitTests.Product.Commands.Create;

using GastroHomeCA.Application.Product.Commands.Create;
using MediatR;
using Xunit;
using Shouldly;

public class CreateProductCommandValidatorTests
{
    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsValid()
    {
        // Arrange
        var validator = new CreateProductCommandValidator();
        var command = new CreateProductCommand { Name = "Product", Category = "Category", Barcode = "12345", CurrentPrice = 10.99m };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameEmpty()
    {
        // Arrange
        var validator = new CreateProductCommandValidator();
        var command = new CreateProductCommand { Name = string.Empty, Category = "Category", Barcode = "12345", CurrentPrice = 10.99m };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].PropertyName.ShouldBe("Name");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameWhitespace()
    {
        // Arrange
        var validator = new CreateProductCommandValidator();
        var command = new CreateProductCommand { Name = "   ", Category = "Category", Barcode = "12345", CurrentPrice = 10.99m };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
    }

    [Fact]
    public void Validate_ShouldFail_WhenCategoryEmpty()
    {
        // Arrange
        var validator = new CreateProductCommandValidator();
        var command = new CreateProductCommand { Name = "Product", Category = string.Empty, Barcode = "12345", CurrentPrice = 10.99m };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].PropertyName.ShouldBe("Category");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPriceNegative()
    {
        // Arrange
        var validator = new CreateProductCommandValidator();
        var command = new CreateProductCommand { Name = "Product", Category = "Category", Barcode = "12345", CurrentPrice = -10.99m };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].PropertyName.ShouldBe("CurrentPrice");
    }

    [Fact]
    public void Validate_ShouldFail_WhenBarcodeInvalid()
    {
        // Arrange
        var validator = new CreateProductCommandValidator();
        var command = new CreateProductCommand { Name = "Product", Category = "Category", Barcode = "invalid@barcode", CurrentPrice = 10.99m };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].PropertyName.ShouldBe("Barcode");
    }

    [Fact]
    public void Validate_ShouldPass_WhenBarcodeNull()
    {
        // Arrange
        var validator = new CreateProductCommandValidator();
        var command = new CreateProductCommand { Name = "Product", Category = "Category", Barcode = null, CurrentPrice = 10.99m };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }
}