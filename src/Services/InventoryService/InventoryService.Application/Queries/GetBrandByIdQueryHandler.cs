using InventoryService.Application.Interfaces;
using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Queries;

public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, Brand?>
{
    private readonly IBrandRepository _brandRepository;

    public GetBrandByIdQueryHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<Brand?> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
        => await _brandRepository.GetByIdAsync(request.Id);
}