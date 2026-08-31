namespace GastroHomeCA.Application.Common.Exceptions;

public class ProductAlreadyExistsException(string? barcode) : Exception($"A product with barcode '{barcode?.ToString() ?? "null"}' already exists.")
{
    public string? Barcode { get; } = barcode;
}