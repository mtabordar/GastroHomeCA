using GastroHomeCA.Application.Product.Commands.Create;
using GastroHomeCA.Application.Product.Commands.Update;
using GastroHomeCA.Application.Product.Queries.GetProductById;
using GastroHomeCA.Application.Common.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GastroHomeCA.Web.Endpoints;

public class Products : IEndpointGroup
{
    public static string? RoutePrefix => null;

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateProduct);
        groupBuilder.MapPut(UpdateProduct, "{id:int}");
        groupBuilder.MapGet("/{id:int}", GetProduct);
    }

    [EndpointSummary("Create a new Product")]
    [EndpointDescription("Creates a new product using the provided details and returns the ID of the created item.")]
    public static async Task<Created<int>> CreateProduct(ISender sender, CreateProductCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/Products/{id}", id);
    }

    [EndpointSummary("Update an existing Product")]
    [EndpointDescription("Updates an existing product with the provided details.")]
    public static async Task<Results<NoContent, BadRequest>> UpdateProduct(ISender sender, int id, UpdateProductCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();

        await sender.Send(command);

        return TypedResults.NoContent();
    }

    [EndpointSummary("Get a Product by ID")]
    [EndpointDescription("Returns a product by its ID.")]
    public static async Task<ProductDto> GetProduct(ISender sender, int id)
    {
        var query = new GetProductByIdQuery(id);
        var product = await sender.Send(query);
        return product;
    }
}