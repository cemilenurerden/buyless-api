using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Queries;

public class GetAllCategoriesQuery : IRequest<List<Category>>
{
}
