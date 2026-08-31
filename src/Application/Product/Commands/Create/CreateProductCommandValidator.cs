namespace GastroHomeCA.Application.Product.Commands.Create;

using FluentValidation;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(255).WithMessage("Name cannot exceed 255 characters");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .MaximumLength(100).WithMessage("Category cannot exceed 100 characters");

        RuleFor(x => x.Barcode)
            .NotEmpty().When(x => !string.IsNullOrEmpty(x.Barcode))
            .Matches(@"^[0-9]+$")
            .WithMessage("Barcode must contain only numbers")
            .MaximumLength(50).WithMessage("Barcode cannot exceed 50 characters");

        RuleFor(x => x.CurrentPrice)
            .GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}