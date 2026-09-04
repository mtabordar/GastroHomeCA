namespace GastroHomeCA.Application.Product.Queries.GetProductById;

using GastroHomeCA.Application.Common.DTOs;
using GastroHomeCA.Application.Common.Exceptions;
using GastroHomeCA.Application.Common.Interfaces;
using GastroHomeCA.Application.Product.Mappings;
using MediatR;

public class GetProductByIdQueryHandler(IApplicationDbContext context) : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
        {
            throw new NotFoundException(request.Id);
        }

        return ProductMappings.ToProductDto(product);
    }
}