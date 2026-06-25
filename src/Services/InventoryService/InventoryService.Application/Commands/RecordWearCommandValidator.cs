using FluentValidation;
using InventoryService.Application.Interfaces;

namespace InventoryService.Application.Commands;

public class RecordWearCommandValidator : AbstractValidator<RecordWearCommand>
{
    private readonly IItemRepository _itemRepository;

    public RecordWearCommandValidator(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;

        RuleFor(x => x.WornDate)
            .LessThanOrEqualTo(DateTime.UtcNow.Date.AddDays(1))
            .WithMessage("Giyilme tarihi gelecekte bir tarih olamaz.");

        RuleFor(x => x)
            .MustAsync(ItemExistsAndBelongsToUser)
            .WithMessage("Belirtilen eşya bulunamadı veya size ait değil.");
    }

    private async Task<bool> ItemExistsAndBelongsToUser(RecordWearCommand command, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(command.ItemId);

        if (item is null)
            return false;

        return item.UserId == command.UserId;
    }
}
