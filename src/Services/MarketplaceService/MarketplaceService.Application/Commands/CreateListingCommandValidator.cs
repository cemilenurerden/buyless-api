using FluentValidation;

namespace MarketplaceService.Application.Commands;

public class CreateListingCommandValidator : AbstractValidator<CreateListingCommand>
{
    public CreateListingCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("İlan başlığı boş olamaz.")
            .MaximumLength(200);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat sıfırdan büyük olmalıdır.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .MaximumLength(3);
    }
}