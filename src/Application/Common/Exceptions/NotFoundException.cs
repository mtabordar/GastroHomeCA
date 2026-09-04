namespace GastroHomeCA.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found.
/// </summary>
public class NotFoundException(int id) : Exception($"Resource with ID {id} not found.")
{
    public int Id { get; } = id;
}