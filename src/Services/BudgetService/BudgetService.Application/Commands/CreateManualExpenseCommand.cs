using MediatR;

namespace BudgetService.Application.Commands;

public class CreateManualExpenseCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly OccurredAt { get; set; }
    public Guid? ItemId { get; set; }
    public int? CategoryId { get; set; }
    public string Currency { get; set; } = "TRY";
    public string? Platform { get; set; }
    public string? Description { get; set; }
}