namespace GastroHomeCA.Application.Product.Commands.Update;

using MediatR;

/// <summary>
/// Command to update an existing product.
/// </summary>
public class UpdateProductCommand : IRequest
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Barcode { get; init; }
    public decimal CurrentPrice { get; init; }
}