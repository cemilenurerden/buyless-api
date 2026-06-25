using FluentValidation;
using InventoryService.Application.Interfaces;

namespace InventoryService.Application.Commands;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategori adı boş olamaz.")
            .MaximumLength(100);

        RuleFor(x => x.Id)
            .MustAsync(CategoryMustExist)
            .WithMessage("Belirtilen kategori bulunamadı.");
    }

    private async Task<bool> CategoryMustExist(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category is not null;
    }
}
