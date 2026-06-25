using FluentValidation;
using InventoryService.Application.Interfaces;

namespace InventoryService.Application.Commands;

public class DeleteBrandCommandValidator : AbstractValidator<DeleteBrandCommand>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IItemRepository _itemRepository;

    public DeleteBrandCommandValidator(
        IBrandRepository brandRepository,
        IItemRepository itemRepository)
    {
        _brandRepository = brandRepository;
        _itemRepository = itemRepository;

        RuleFor(x => x.Id)
            .MustAsync(BrandMustExist)
            .WithMessage("Belirtilen marka bulunamadı.");

        // Not: Brand-Item ilişkisi DeleteBehavior.SetNull olduğu için veritabanı seviyesinde
        // engellenmiyor (marka silinince Item'ın BrandId'si null'a düşer, Item silinmez).
        // Ama kullanıcıya bunu bilerek mi yaptığını sormak için burada da kontrol ediyoruz -
        // marka kullanımdaysa silmeyi reddedip "X eşyada kullanılıyor" diye uyarıyoruz.
        RuleFor(x => x.Id)
            .MustAsync(BrandMustNotBeInUse)
            .WithMessage("Bu marka eşyalarda kullanılıyor, silinemez. Önce o eşyaların markasını değiştirin.")
            .When(x => x.Id > 0);
    }

    private async Task<bool> BrandMustExist(int id, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        return brand is not null;
    }

    private async Task<bool> BrandMustNotBeInUse(int id, CancellationToken cancellationToken)
        => !await _itemRepository.ExistsByBrandIdAsync(id);
}
