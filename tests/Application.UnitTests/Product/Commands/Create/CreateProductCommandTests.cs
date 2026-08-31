namespace GastroHomeCA.Application.UnitTests.Product.Commands.Create;

using GastroHomeCA.Application.Product.Commands.Create;
using GastroHomeCA.Application.Common.Interfaces;
using GastroHomeCA.Domain.Entities;
using Xunit;
using Shouldly;
using Moq;

public class CreateProductCommandTests
{
    [Fact]
    public async Task Handle_ShouldReturnNewProductId_WhenValidCommand()
    {
        // Arrange
        var mockDbContext = new Mock<IApplicationDbContext>();
        mockDbContext.Setup(x => x.Products.Add(It.IsAny<Product>()))
            .Verifiable();
        mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new GastroHomeCA.Application.Product.Commands.Create.CreateProductCommandHandler(mockDbContext.Object);
        var command = new CreateProductCommand { Name = "Test Product", Category = "Electronics", Barcode = "1234567", CurrentPrice = 29.99m };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBeGreaterThan(0);
        mockDbContext.Verify(x => x.Products.Add(It.IsAny<Product>()), Times.Once);
        mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenCommandValidationFails()
    {
        // Arrange
        var mockDbContext = new Mock<IApplicationDbContext>();
        var handler = new GastroHomeCA.Application.Product.Commands.Create.CreateProductCommandHandler(mockDbContext.Object);
        var command = new CreateProductCommand { Name = string.Empty, Category = "Category", Barcode = "12345", CurrentPrice = 10.99m };

        // Act & Assert
        var exception = await Record.ExceptionAsync(async () => 
            await handler.Handle(command, CancellationToken.None));

        exception.ShouldBeOfType<GastroHomeCA.Application.Common.Exceptions.ValidationException>();
    }

    [Fact]
    public async Task Handle_ShouldCreateProductWithCorrectData()
    {
        // Arrange
        var mockDbContext = new Mock<IApplicationDbContext>();
        mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new GastroHomeCA.Application.Product.Commands.Create.CreateProductCommandHandler(mockDbContext.Object);
        var command = new CreateProductCommand { Name = "Product Name", Category = "Electronics", Barcode = "1234567", CurrentPrice = 29.99m };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockDbContext.Verify(x => x.Products.Add(It.Is<Product>(p =>
            p.Name == "Product Name" &&
            p.Category == "Electronics" &&
            p.Barcode == "1234567" &&
            p.CurrentPrice == 29.99m)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateProduct_WhenBarcodeIsNull()
    {
        // Arrange
        var mockDbContext = new Mock<IApplicationDbContext>();
        mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new GastroHomeCA.Application.Product.Commands.Create.CreateProductCommandHandler(mockDbContext.Object);
        var command = new CreateProductCommand { Name = "Product", Category = "Category", Barcode = null, CurrentPrice = 10.99m };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockDbContext.Verify(x => x.Products.Add(It.Is<Product>(p => p.Barcode == null)), Times.Once);
    }
}