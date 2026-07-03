using MediatR;

namespace MarketplaceService.Application.Commands;

public class PurchaseListingCommand : IRequest<Guid>
{
    public Guid ListingId { get; set; }
    public Guid BuyerId { get; set; }
    public string? ShippingAddress { get; set; }
    public string AccessToken { get; set; } = null!; // InventoryService'e iletmek için
}