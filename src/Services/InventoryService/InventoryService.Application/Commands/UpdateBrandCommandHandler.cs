using InventoryService.Application.Interfaces;
using MediatR;

namespace InventoryService.Application.Commands;

public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand>
{
    private readonly IBrandRepository _brandRepository;

    public UpdateBrandCommandHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(request.Id);

        if (brand is null)
            throw new InvalidOperationException("Belirtilen marka bulunamadı.");

        brand.UpdateDetails(request.Name, request.LogoUrl);

        await _brandRepository.UpdateAsync(brand);
    }
}