namespace GastroHomeCA.Application.Product.Mappings;

using GastroHomeCA.Application.Common.DTOs;
using GastroHomeCA.Domain.Entities;

/// <summary>
/// Maps between Product entities and DTOs.
/// </summary>
public static class ProductMappings
{
    public static CreateProductDto ToCreateProductDto(Guid id, Product product) => new()
    {
        Id = id,
        Name = product.Name,
        Category = product.Category,
        Barcode = product.Barcode,
        CurrentPrice = product.CurrentPrice
    };
}
