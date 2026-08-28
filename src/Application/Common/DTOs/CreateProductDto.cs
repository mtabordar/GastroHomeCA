namespace GastroHomeCA.Application.Common.DTOs;

/// <summary>
/// Data transfer object for creating a new product.
/// </summary>
public class CreateProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public decimal CurrentPrice { get; init; }
}
