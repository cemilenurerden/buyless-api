using FluentValidation;
using InventoryService.Application.Interfaces;

namespace InventoryService.Application.Commands;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategori adı boş olamaz.")
            .MaximumLength(100);

        RuleFor(x => x.ParentCategoryId)
            .MustAsync(ParentMustExistAndBeTopLevel)
            .When(x => x.ParentCategoryId.HasValue)
            .WithMessage("Belirtilen üst kategori bulunamadı veya kendisi zaten bir alt kategori (2 seviye sınırı aşılamaz).");
    }

    private async Task<bool> ParentMustExistAndBeTopLevel(int? parentCategoryId, CancellationToken cancellationToken)
    {
        var parent = await _categoryRepository.GetByIdAsync(parentCategoryId!.Value);

        if (parent is null)
            return false;

        // 2 seviye sınırı: bir alt kategorinin kendi alt kategorisi olamaz.
        // Yani seçilen "parent", kendisi zaten bir ParentCategoryId'ye sahipse (yani bir alt kategoriyse), reddet.
        return parent.ParentCategoryId is null;
    }
}