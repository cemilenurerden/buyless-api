using MediatR;

namespace InventoryService.Application.Commands;

public class UpdateBrandCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? LogoUrl { get; set; }
}