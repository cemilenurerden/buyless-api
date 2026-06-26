using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Queries;

public class GetItemByIdQuery : IRequest<Item?>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}