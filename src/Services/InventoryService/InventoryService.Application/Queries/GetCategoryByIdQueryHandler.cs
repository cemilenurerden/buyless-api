using InventoryService.Application.Interfaces;
using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Queries;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Category?>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Category?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        => await _categoryRepository.GetByIdAsync(request.Id);
}