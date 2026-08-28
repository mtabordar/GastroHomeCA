namespace GastroHomeCA.Application.Product.Handlers.Create;

using GastroHomeCA.Application.Common.Interfaces;
using GastroHomeCA.Application.Product.Commands.Create;
using GastroHomeCA.Domain.Entities;

/// <summary>
/// Handles the creation of a new product.
/// </summary>
public class CreateProductCommandHandler(IApplicationDbContext dbContext) : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product();
        product.Create(request.Name, request.Category, request.Barcode, request.CurrentPrice);
        await dbContext.Products.AddAsync(product, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return (int)product.Id;
    }
}
