using MediatR;

namespace InventoryService.Application.Commands;

public class DeleteBrandCommand : IRequest
{
    public int Id { get; set; }
}