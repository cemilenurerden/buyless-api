using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Queries;

public class GetUserItemsQuery : IRequest<List<Item>>
{
    public Guid UserId { get; set; }
}
