using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Queries;

public class GetBarcodeProductQuery : IRequest<BarcodeProductCache?>
{
    public string Barcode { get; set; } = null!;
}