namespace GastroHomeCA.Application.Product.Commands.Create;

using MediatR;

/// <summary>
/// Command to create a new product.
/// </summary>
public class CreateProductCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public string? Barcode { get; init; }

    public decimal CurrentPrice { get; init; }
}
