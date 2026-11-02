namespace GastroHomeCA.Application.Product.Commands.Delete;

using GastroHomeCA.Application.Common.Exceptions;
using GastroHomeCA.Application.Common.Interfaces;
using MediatR;

public class DeleteProductCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException(request.Id);
        }

        product.Delete();
        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);
    }
}
