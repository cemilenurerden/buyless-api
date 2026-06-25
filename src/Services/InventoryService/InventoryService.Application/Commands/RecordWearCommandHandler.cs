using InventoryService.Application.Interfaces;
using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Commands;

public class RecordWearCommandHandler : IRequestHandler<RecordWearCommand, RecordWearResult>
{
    private readonly IItemRepository _itemRepository;
    private readonly IItemWearLogRepository _itemWearLogRepository;

    public RecordWearCommandHandler(
        IItemRepository itemRepository,
        IItemWearLogRepository itemWearLogRepository)
    {
        _itemRepository = itemRepository;
        _itemWearLogRepository = itemWearLogRepository;
    }

    public async Task<RecordWearResult> Handle(RecordWearCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(request.ItemId);

        if (item is null || item.UserId != request.UserId)
            throw new InvalidOperationException("Belirtilen eşya bulunamadı veya size ait değil.");

        // Domain metodu: WornCount'u artırır, LastWornDate'i günceller.
        // Item zaten Active değilse (Donated/Sold/Recycled) burada exception fırlatır.
        item.RecordWear(request.WornDate);

        var wearLog = new ItemWearLog(request.ItemId, request.UserId, request.WornDate);

        await _itemWearLogRepository.AddAsync(wearLog);
        await _itemRepository.UpdateAsync(item);

        return new RecordWearResult
        {
            WearLogId = wearLog.Id,
            NewWornCount = item.WornCount
        };
    }
}
