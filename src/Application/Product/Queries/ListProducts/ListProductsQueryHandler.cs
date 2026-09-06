using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using GastroHomeCA.Domain.Entities;
using GastroHomeCA.Application.Common.DTOs;
using GastroHomeCA.Application.Product.Queries.ListProducts;
using GastroHomeCA.Application.Product.Mappings;
using GastroHomeCA.Application.Common.Interfaces;

namespace GastroHomeCA.Application.Product.Queries.ListProducts;

/// <summary>
/// Handler to retrieve all products.
/// </summary>
public class ListProductsQueryHandler(IApplicationDbContext context) : IRequestHandler<ListProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await context.Products.ToListAsync(cancellationToken);
        return products.Select(ProductMappings.ToProductDto).ToList();
    }
}
