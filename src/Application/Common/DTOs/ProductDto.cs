namespace GastroHomeCA.Application.Common.DTOs;

/// <summary>
/// Data transfer object for reading a product entity.
/// </summary>
public class ProductDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public decimal CurrentPrice { get; init; }
    public DateTime CreatedDate { get; init; }
    public DateTime LastUpdatedDate { get; init; }
}