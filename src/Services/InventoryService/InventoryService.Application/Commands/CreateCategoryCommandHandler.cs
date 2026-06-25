using InventoryService.Application.Interfaces;
using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Commands;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category(request.Name, request.ParentCategoryId, request.IconUrl);

        await _categoryRepository.AddAsync(category);

        return category.Id;
    }
}
