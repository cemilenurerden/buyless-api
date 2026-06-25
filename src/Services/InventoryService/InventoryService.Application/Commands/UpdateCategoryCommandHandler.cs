using InventoryService.Application.Interfaces;
using MediatR;

namespace InventoryService.Application.Commands;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id);

        if (category is null)
            throw new InvalidOperationException("Belirtilen kategori bulunamadı.");

        category.UpdateDetails(request.Name, request.IconUrl);

        await _categoryRepository.UpdateAsync(category);
    }
}
