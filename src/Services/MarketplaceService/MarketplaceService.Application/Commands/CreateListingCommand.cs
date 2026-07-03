using BuyLess.Shared;
using MediatR;

namespace MarketplaceService.Application.Commands;

public class CreateListingCommand : IRequest<Guid>
{
    public Guid SellerId { get; set; }
    public Guid? ItemId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "TRY";
    public ItemCondition Condition { get; set; }
}