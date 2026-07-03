using MarketplaceService.Application.Interfaces;
using MarketplaceService.Domain;
using MediatR;

namespace MarketplaceService.Application.Commands;

public class RespondToOfferCommand : IRequest<Guid?>
{
    public Guid OfferId { get; set; }
    public Guid SellerId { get; set; }
    public bool Accept { get; set; }
    public string AccessToken { get; set; } = null!;
}

public class RespondToOfferCommandHandler : IRequestHandler<RespondToOfferCommand, Guid?>
{
    private readonly IOfferRepository _offerRepository;
    private readonly IListingRepository _listingRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryServiceClient _inventoryServiceClient;

    public RespondToOfferCommandHandler(
        IOfferRepository offerRepository,
        IListingRepository listingRepository,
        IOrderRepository orderRepository,
        IInventoryServiceClient inventoryServiceClient)
    {
        _offerRepository = offerRepository;
        _listingRepository = listingRepository;
        _orderRepository = orderRepository;
        _inventoryServiceClient = inventoryServiceClient;
    }

    public async Task<Guid?> Handle(RespondToOfferCommand request, CancellationToken cancellationToken)
    {
        var offer = await _offerRepository.GetByIdAsync(request.OfferId);

        if (offer is null)
            throw new InvalidOperationException("Belirtilen teklif bulunamadı.");

        var listing = await _listingRepository.GetByIdAsync(offer.ListingId);

        if (listing is null)
            throw new InvalidOperationException("İlana ait ilan bulunamadı.");

        // Sadece satıcı yanıt verebilir
        if (listing.SellerId != request.SellerId)
            throw new InvalidOperationException("Bu teklife yanıt verme yetkiniz yok.");

        if (!request.Accept)
        {
            offer.Reject();
            await _offerRepository.UpdateAsync(offer);
            return null;
        }

        // Teklif kabul edildi
        offer.Accept();
        await _offerRepository.UpdateAsync(offer);

        // İlanı Sold yap
        listing.MarkAsSold();
        await _listingRepository.UpdateAsync(listing);

        // Sipariş oluştur (teklif fiyatıyla)
        var order = new Order(listing.Id, offer.BuyerId, listing.SellerId, offer.OfferedPrice, listing.Currency);
        await _orderRepository.AddAsync(order);

        // InventoryService'e bildirim
        if (listing.ItemId.HasValue)
        {
            await _inventoryServiceClient.NotifySoldAsync(
                listing.ItemId.Value,
                offer.OfferedPrice,
                request.AccessToken);
        }

        return order.Id;
    }
}