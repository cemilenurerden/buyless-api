using FluentValidation;
using InventoryService.Application.Interfaces;

namespace InventoryService.Application.Commands;

public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
{
    private readonly IBrandRepository _brandRepository;

    public UpdateBrandCommandValidator(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Marka adı boş olamaz.")
            .MaximumLength(100);

        RuleFor(x => x.Id)
            .MustAsync(BrandMustExist)
            .WithMessage("Belirtilen marka bulunamadı.");
    }

    private async Task<bool> BrandMustExist(int id, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        return brand is not null;
    }
}
