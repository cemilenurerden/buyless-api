using FluentValidation;
using InventoryService.Application.Interfaces;

namespace InventoryService.Application.Commands;

public class AddItemCommandValidator : AbstractValidator<AddItemCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;

    public AddItemCommandValidator(
        ICategoryRepository categoryRepository,
        IBrandRepository brandRepository)
    {
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Eşya adı boş olamaz.")
            .MaximumLength(200).WithMessage("Eşya adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.PurchasePrice)
            .GreaterThanOrEqualTo(0).WithMessage("Satın alma fiyatı negatif olamaz.");

        RuleFor(x => x.PurchaseDate)
            .NotEmpty().WithMessage("Satın alma tarihi belirtilmelidir.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Satın alma tarihi gelecekte olamaz.");

        // CategoryId veritabanında gerçekten var mı? Yoksa Item, olmayan bir kategoriye bağlanır.
        RuleFor(x => x.CategoryId)
            .MustAsync(async (categoryId, cancellation) =>
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                return category is not null;
            })
            .WithMessage("Belirtilen kategori bulunamadı.");

        // BrandId opsiyonel (nullable) - sadece değer verilmişse kontrol ediyoruz.
        RuleFor(x => x.BrandId)
            .MustAsync(async (brandId, cancellation) =>
            {
                if (brandId is null) return true;
                var brand = await _brandRepository.GetByIdAsync(brandId.Value);
                return brand is not null;
            })
            .WithMessage("Belirtilen marka bulunamadı.");
    }
}