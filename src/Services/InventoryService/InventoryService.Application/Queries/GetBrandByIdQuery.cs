using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Queries;

public class GetBrandByIdQuery : IRequest<Brand?>
{
    public int Id { get; set; }
}
