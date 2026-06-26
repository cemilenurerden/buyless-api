using MediatR;

namespace InventoryService.Application.Commands;

public class RecycleItemCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}