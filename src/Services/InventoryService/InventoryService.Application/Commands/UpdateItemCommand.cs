using MediatR;

namespace InventoryService.Application.Commands;

public class UpdateItemCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Color { get; set; }
    public string? Size { get; set; }
    public string? ImageUrl { get; set; }
    public string Condition { get; set; } = null!;
    public string? Notes { get; set; }
}