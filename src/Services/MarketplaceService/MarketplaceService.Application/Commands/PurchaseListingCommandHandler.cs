using MarketplaceService.Application.Interfaces;
using MarketplaceService.Domain;
using MediatR;

namespace MarketplaceService.Application.Commands;

public class PurchaseListingCommandHandler : IRequestHandler<PurchaseListingCommand, Guid>
{
    private readonly IListingRepository _listingRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryServiceClient _inventoryServiceClient;

    public PurchaseListingCommandHandler(
        IListingRepository listingRepository,
        IOrderRepository orderRepository,
        IInventoryServiceClient inventoryServiceClient)
    {
        _listingRepository = listingRepository;
        _orderRepository = orderRepository;
        _inventoryServiceClient = inventoryServiceClient;
    }

    public async Task<Guid> Handle(PurchaseListingCommand request, CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetByIdAsync(request.ListingId);

        if (listing is null)
            throw new InvalidOperationException("Belirtilen ilan bulunamadı.");

        if (listing.Status != ListingStatus.Active)
            throw new InvalidOperationException("Bu ilan artık aktif değil, satın alınamaz.");

        // Kullanıcı kendi ilanını satın alamaz
        if (listing.SellerId == request.BuyerId)
            throw new InvalidOperationException("Kendi ilanınızı satın alamazsınız.");

        // 1. İlanı "Sold" yap
        listing.MarkAsSold();
        await _listingRepository.UpdateAsync(listing);

        // 2. Sipariş oluştur
        var order = new Order(
            listing.Id,
            request.BuyerId,
            listing.SellerId,
            listing.Price,
            listing.Currency);

        if (!string.IsNullOrWhiteSpace(request.ShippingAddress))
            order.SetShippingAddress(request.ShippingAddress);

        await _orderRepository.AddAsync(order);

        // 3. InventoryService'e bildirim (eğer ilan bir Item'a bağlıysa)
        // Hata olursa sessizce loglanır, satış işlemini engellemez.
        if (listing.ItemId.HasValue)
        {
            await _inventoryServiceClient.NotifySoldAsync(
                listing.ItemId.Value,
                listing.Price,
                request.AccessToken);
        }

        return order.Id;
    }
}