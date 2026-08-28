namespace GastroHomeCA.Application.Product.Mappings;

using GastroHomeCA.Application.Common.DTOs;
using GastroHomeCA.Domain.Entities;

/// <summary>
/// Maps between Product entities and DTOs.
/// </summary>
public static class ProductMappings
{
    public static CreateProductDto ToCreateProductDto(Product product) => new()
    {
        Name = product.Name,
        Category = product.Category,
        Barcode = product.Barcode,
        CurrentPrice = product.CurrentPrice
    };

    public static ProductDto ToProductDto(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Category = product.Category,
        Barcode = product.Barcode,
        CurrentPrice = product.CurrentPrice,
        CreatedDate = product.CreatedDate,
        LastUpdatedDate = product.LastUpdatedDate
    };
}
