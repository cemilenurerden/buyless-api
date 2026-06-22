using InventoryService.Application.Interfaces;
using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Commands;

public class AddItemCommandHandler : IRequestHandler<AddItemCommand, Guid>
{
    private readonly IItemRepository _itemRepository;

    public AddItemCommandHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Guid> Handle(AddItemCommand request, CancellationToken cancellationToken)
    {
        // Validation (CategoryId/BrandId var mı, fiyat negatif mi vb.) ValidationBehavior tarafında
        // bu Handler'a gelmeden önce zaten yapıldı. Burada sadece Domain nesnesini oluşturup kaydediyoruz.
        var item = new Item(
            userId: request.UserId,
            categoryId: request.CategoryId,
            name: request.Name,
            purchasePrice: request.PurchasePrice,
            purchaseDate: request.PurchaseDate,
            brandId: request.BrandId,
            color: request.Color,
            size: request.Size,
            purchaseSource: request.PurchaseSource,
            barcode: request.Barcode,
            imageUrl: request.ImageUrl,
            condition: request.Condition,
            notes: request.Notes
        );

        await _itemRepository.AddAsync(item);

        return item.Id;
    }
}