using MediatR;

namespace InventoryService.Application.Commands;

public class CreateCategoryCommand : IRequest<int>
{
    public string Name { get; set; } = null!;
    public int? ParentCategoryId { get; set; }
    public string? IconUrl { get; set; }
}