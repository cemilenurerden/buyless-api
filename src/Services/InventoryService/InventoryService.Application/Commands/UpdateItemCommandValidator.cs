using FluentValidation;
using InventoryService.Application.Interfaces;
using InventoryService.Domain;

namespace InventoryService.Application.Commands;

public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
{
    private readonly IItemRepository _itemRepository;

    public UpdateItemCommandValidator(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Eşya adı boş olamaz.")
            .MaximumLength(200);

        RuleFor(x => x.Condition)
            .Must(c => Enum.TryParse<ItemCondition>(c, out _))
            .WithMessage("Condition değeri geçersiz. Geçerli değerler: New, Good, Fair, Poor.");

        RuleFor(x => x)
            .MustAsync(ItemMustExistAndBelongToUser)
            .WithMessage("Belirtilen eşya bulunamadı veya size ait değil.");
    }

    private async Task<bool> ItemMustExistAndBelongToUser(UpdateItemCommand command, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(command.Id);

        if (item is null)
            return false;

        return item.UserId == command.UserId;
    }
}