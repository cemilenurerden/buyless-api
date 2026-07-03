using MarketplaceService.Application.Interfaces;
using MarketplaceService.Domain;
using MediatR;

namespace MarketplaceService.Application.Commands;

public class CreateListingCommandHandler : IRequestHandler<CreateListingCommand, Guid>
{
    private readonly IListingRepository _listingRepository;

    public CreateListingCommandHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }

    public async Task<Guid> Handle(CreateListingCommand request, CancellationToken cancellationToken)
    {
        var listing = new Listing(
            request.SellerId,
            request.Title,
            request.Price,
            request.Condition,
            request.ItemId,
            request.Description,
            request.Currency);

        await _listingRepository.AddAsync(listing);

        return listing.Id;
    }
}