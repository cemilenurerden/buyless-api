using MediatR;

namespace InventoryService.Application.Commands;

public class DonateItemCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    // Controller'dan gelen orijinal JWT - BudgetService'e bildirim gönderirken
    // isteği yapan kullanıcının kimliğini taşımak için aynen iletilir.
    public string AccessToken { get; set; } = null!;
}