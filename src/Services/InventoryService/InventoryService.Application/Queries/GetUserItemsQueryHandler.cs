using InventoryService.Application.Interfaces;
using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Queries;

public class GetUserItemsQueryHandler : IRequestHandler<GetUserItemsQuery, List<Item>>
{
    private readonly IItemRepository _itemRepository;

    public GetUserItemsQueryHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<List<Item>> Handle(GetUserItemsQuery request, CancellationToken cancellationToken)
        => await _itemRepository.GetByUserIdAsync(request.UserId);
}
