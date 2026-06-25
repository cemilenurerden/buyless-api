using FluentValidation;

namespace InventoryService.Application.Commands;

public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Marka adı boş olamaz.")
            .MaximumLength(100);
    }
}