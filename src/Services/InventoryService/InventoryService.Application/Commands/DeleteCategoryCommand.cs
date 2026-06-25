using MediatR;

namespace InventoryService.Application.Commands;

public class DeleteCategoryCommand : IRequest
{
    public int Id { get; set; }
}
