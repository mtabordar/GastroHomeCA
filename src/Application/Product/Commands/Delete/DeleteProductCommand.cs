namespace GastroHomeCA.Application.Product.Commands.Delete;

using MediatR;

/// <summary>
/// Command to delete an existing product.
/// </summary>
public class DeleteProductCommand : IRequest
{
    public int Id { get; init; }
}
