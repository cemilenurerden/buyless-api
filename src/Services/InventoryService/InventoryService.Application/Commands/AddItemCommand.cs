using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Commands;

public class AddItemCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }          // JWT'den okunacak, client göndermeyecek
    public int CategoryId { get; set; }
    public int? BrandId { get; set; }
    public string Name { get; set; } = null!;
    public string? Color { get; set; }
    public string? Size { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string? PurchaseSource { get; set; }
    public string? Barcode { get; set; }
    public string? ImageUrl { get; set; }
    public ItemCondition Condition { get; set; } = ItemCondition.New;
    public string? Notes { get; set; }
}