using FluentValidation;
using MarketplaceService.Application.Interfaces;
using MarketplaceService.Domain;
using MediatR;

namespace MarketplaceService.Application.Commands;

public class MakeOfferCommand : IRequest<Guid>
{
    public Guid ListingId { get; set; }
    public Guid BuyerId { get; set; }
    public decimal OfferedPrice { get; set; }
    public string? Message { get; set; }
}

public class MakeOfferCommandValidator : AbstractValidator<MakeOfferCommand>
{
    public MakeOfferCommandValidator()
    {
        RuleFor(x => x.OfferedPrice)
            .GreaterThan(0).WithMessage("Teklif fiyatı sıfırdan büyük olmalıdır.");
    }
}

public class MakeOfferCommandHandler : IRequestHandler<MakeOfferCommand, Guid>
{
    private readonly IListingRepository _listingRepository;
    private readonly IOfferRepository _offerRepository;

    public MakeOfferCommandHandler(IListingRepository listingRepository, IOfferRepository offerRepository)
    {
        _listingRepository = listingRepository;
        _offerRepository = offerRepository;
    }

    public async Task<Guid> Handle(MakeOfferCommand request, CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByIdAsync(request.ListingId);

        if (listing is null)
            throw new InvalidOperationException("Belirtilen ilan bulunamadı.");

        if (listing.Status != ListingStatus.Active)
            throw new InvalidOperationException("Bu ilan artık aktif değil.");

        if (listing.SellerId == request.BuyerId)
            throw new InvalidOperationException("Kendi ilanınıza teklif veremezsiniz.");

        var offer = new Offer(request.ListingId, request.BuyerId, request.OfferedPrice, request.Message);

        await _offerRepository.AddAsync(offer);

        return offer.Id;
    }
}