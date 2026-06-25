using FluentValidation;
using InventoryService.Application.Interfaces;

namespace InventoryService.Application.Commands;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IItemRepository _itemRepository;

    public DeleteCategoryCommandValidator(
        ICategoryRepository categoryRepository,
        IItemRepository itemRepository)
    {
        _categoryRepository = categoryRepository;
        _itemRepository = itemRepository;

        RuleFor(x => x.Id)
            .MustAsync(CategoryMustExist)
            .WithMessage("Belirtilen kategori bulunamadı.");

        RuleFor(x => x.Id)
            .MustAsync(CategoryMustNotHaveChildren)
            .WithMessage("Bu kategorinin alt kategorileri var, önce onları silin veya taşıyın.")
            .When(x => x.Id > 0);

        RuleFor(x => x.Id)
            .MustAsync(CategoryMustNotHaveItems)
            .WithMessage("Bu kategoriye ait eşyalar var, kategori silinemez.")
            .When(x => x.Id > 0);
    }

    private async Task<bool> CategoryMustExist(int id, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category is not null;
    }

    private async Task<bool> CategoryMustNotHaveChildren(int id, CancellationToken cancellationToken)
        => !await _categoryRepository.HasChildCategoriesAsync(id);

    private async Task<bool> CategoryMustNotHaveItems(int id, CancellationToken cancellationToken)
        => !await _itemRepository.ExistsByCategoryIdAsync(id);
}
