using MediatR;

namespace InventoryService.Application.Commands;

public class UpdateCategoryCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? IconUrl { get; set; }
}