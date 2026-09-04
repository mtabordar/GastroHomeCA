namespace GastroHomeCA.Application.Product.Queries.GetProductById;

using GastroHomeCA.Application.Common.DTOs;
using MediatR;

/// <summary>
/// Query to retrieve a product by its ID.
/// </summary>
public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;
