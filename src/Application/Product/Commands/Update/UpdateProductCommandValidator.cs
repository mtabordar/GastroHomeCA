namespace GastroHomeCA.Application.Product.Commands.Update;

using FluentValidation;

/// <summary>
/// Validates the UpdateProductCommand before it's sent.
/// </summary>
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Product ID must be greater than zero.")
            .Must(x => x > 0).WithMessage("Invalid product ID.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MaximumLength(200).WithMessage("Category must not exceed 200 characters.");

        RuleFor(x => x.CurrentPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to zero.");
    }
}