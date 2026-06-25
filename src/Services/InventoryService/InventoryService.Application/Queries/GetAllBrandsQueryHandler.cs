using InventoryService.Application.Interfaces;
using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Queries;

public class GetAllBrandsQueryHandler : IRequestHandler<GetAllBrandsQuery, List<Brand>>
{
    private readonly IBrandRepository _brandRepository;

    public GetAllBrandsQueryHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<List<Brand>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        => await _brandRepository.GetAllAsync();
}