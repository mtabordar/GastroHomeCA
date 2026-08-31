namespace GastroHomeCA.Application.Product.Commands.Create;

using GastroHomeCA.Application.Common.Exceptions;
using GastroHomeCA.Application.Common.Interfaces;
using GastroHomeCA.Domain.Entities;
using MediatR;

//

public class CreateProductCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var existingProduct = await context.Products
            .FirstOrDefaultAsync(p => p.Barcode == request.Barcode, cancellationToken);

        if (existingProduct != null)
        {
            throw new ProductAlreadyExistsException(request.Barcode);
        }

        var product = new Product();
        product.Create(request.Name, request.Category, request.Barcode, request.CurrentPrice);
        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}