using InventoryService.Application.Interfaces;
using MediatR;

namespace InventoryService.Application.Commands;

public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand>
{
    private readonly IBrandRepository _brandRepository;

    public DeleteBrandCommandHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(request.Id);

        if (brand is null)
            throw new InvalidOperationException("Belirtilen marka bulunamadı.");

        await _brandRepository.DeleteAsync(brand);
    }
}