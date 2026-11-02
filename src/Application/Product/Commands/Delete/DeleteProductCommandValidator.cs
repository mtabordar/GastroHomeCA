namespace GastroHomeCA.Application.Product.Commands.Delete;

using FluentValidation;

/// <summary>
/// Validates the DeleteProductCommand before it's sent.
/// </summary>
public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Product ID must be greater than zero.");
    }
}
