using InventoryService.Application.Interfaces;
using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Commands;

public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, int>
{
    private readonly IBrandRepository _brandRepository;

    public CreateBrandCommandHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<int> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = new Brand(request.Name, request.LogoUrl, request.IsVerified);

        await _brandRepository.AddAsync(brand);

        return brand.Id;
    }
}
