using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using GastroHomeCA.Domain.Entities;
using GastroHomeCA.Application.Common.DTOs;

namespace GastroHomeCA.Application.Product.Queries.ListProducts;

/// <summary>
/// Query to retrieve all products.
/// </summary>
public class ListProductsQuery : IRequest<List<ProductDto>>
{
    public ListProductsQuery() { }
}
