using MarketplaceService.Application.Interfaces;
using MediatR;

namespace MarketplaceService.Application.Commands;

public class AddListingImageCommand : IRequest
{
    public Guid ListingId { get; set; }
    public Guid SellerId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public int DisplayOrder { get; set; }
}

public class AddListingImageCommandHandler : IRequestHandler<AddListingImageCommand>
{
    private readonly IListingRepository _listingRepository;

    public AddListingImageCommandHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public async Task Handle(AddListingImageCommand request, CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByIdAsync(request.ListingId);

        if (listing is null)
            throw new InvalidOperationException("Belirtilen ilan bulunamadı.");

        if (listing.SellerId != request.SellerId)
            throw new InvalidOperationException("Bu ilana fotoğraf ekleme yetkiniz yok.");

        listing.AddImage(request.ImageUrl, request.DisplayOrder);

        await _listingRepository.UpdateAsync(listing);
    }
}