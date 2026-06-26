using InventoryService.Application.Interfaces;
using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Queries;

public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, Item?>
{
    private readonly IItemRepository _itemRepository;

    public GetItemByIdQueryHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Item?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(request.Id);

        // Eşya yoksa ya da başka bir kullanıcıya aitse, null döndürüyoruz -
        // Controller bunu 404 olarak yorumlayacak (varlığını ifşa etmemek için
        // "bu sana ait değil" yerine "bulunamadı" demek daha güvenli).
        if (item is null || item.UserId != request.UserId)
            return null;

        return item;
    }
}