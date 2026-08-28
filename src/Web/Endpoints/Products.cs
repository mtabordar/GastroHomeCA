namespace GastroHomeCA.Web.Endpoints;

using GastroHomeCA.Application.Product.Commands.Create;
using Microsoft.AspNetCore.Http.HttpResults;

public class Products : IEndpointGroup
{
    public static string? RoutePrefix => null;

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateProduct);
    }

    [EndpointSummary("Create a new Product")]
    [EndpointDescription("Creates a new product using the provided details and returns the ID of the created item.")]
    public static async Task<Created<int>> CreateProduct(ISender sender, CreateProductCommand command)
    {
        var id = await sender.Send(command);

        return TypedResults.Created($"/api/Products/{id}", id);
    }
}