using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Queries;

public class GetAllBrandsQuery : IRequest<List<Brand>>
{
}