using MediatR;

namespace InventoryService.Application.Commands;

public class CreateBrandCommand : IRequest<int>
{
    public string Name { get; set; } = null!;
    public string? LogoUrl { get; set; }
    public bool IsVerified { get; set; }
}