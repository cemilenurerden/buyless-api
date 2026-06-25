using InventoryService.Application.Interfaces;
using MediatR;

namespace InventoryService.Application.Commands;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id);

        if (category is null)
            throw new InvalidOperationException("Belirtilen kategori bulunamadı.");

        await _categoryRepository.DeleteAsync(category);
    }
}
