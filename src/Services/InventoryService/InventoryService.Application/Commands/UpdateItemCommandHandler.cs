using InventoryService.Application.Interfaces;
using InventoryService.Domain;
using MediatR;
using BuyLess.Shared;

namespace InventoryService.Application.Commands;

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand>
{
    private readonly IItemRepository _itemRepository;

    public UpdateItemCommandHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(request.Id);

        if (item is null || item.UserId != request.UserId)
            throw new InvalidOperationException("Belirtilen eşya bulunamadı veya size ait değil.");

        var condition = Enum.Parse<ItemCondition>(request.Condition);

        item.UpdateDetails(request.Name, request.Color, request.Size, request.ImageUrl, condition, request.Notes);

        await _itemRepository.UpdateAsync(item);
    }
}