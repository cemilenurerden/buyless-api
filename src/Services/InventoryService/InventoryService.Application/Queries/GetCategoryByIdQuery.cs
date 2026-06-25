using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Queries;

public class GetCategoryByIdQuery : IRequest<Category?>
{
    public int Id { get; set; }
}
