using InventoryService.Application.Interfaces;
using MediatR;

namespace InventoryService.Application.Commands;

public class RecycleItemCommandHandler : IRequestHandler<RecycleItemCommand>
{
    private readonly IItemRepository _itemRepository;

    public RecycleItemCommandHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task Handle(RecycleItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(request.Id);

        if (item is null || item.UserId != request.UserId)
            throw new InvalidOperationException("Belirtilen eşya bulunamadı veya size ait değil.");

        item.MarkAsRecycled();

        await _itemRepository.UpdateAsync(item);
    }
}