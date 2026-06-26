using MediatR;

namespace InventoryService.Application.Commands;

public class SellItemCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
