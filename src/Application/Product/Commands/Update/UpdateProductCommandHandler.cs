namespace GastroHomeCA.Application.Product.Commands.Update;

using GastroHomeCA.Application.Common.Exceptions;
using GastroHomeCA.Application.Common.Interfaces;
using MediatR;

public class UpdateProductCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateProductCommand>
{
    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var existingProduct = await context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (existingProduct == null)
        {
            throw new NotFoundException(request.Id);
        }

        // Update entity properties (domain validation will catch empty values)
        existingProduct.Update(request.Name, request.Category, request.Barcode);
        existingProduct.UpdatePrice(request.CurrentPrice);

        await context.SaveChangesAsync(cancellationToken);
    }
}