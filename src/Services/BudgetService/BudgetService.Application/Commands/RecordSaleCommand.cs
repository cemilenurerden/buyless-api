using MediatR;

namespace BudgetService.Application.Commands;

public class RecordSaleCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Guid ItemId { get; set; }
    public decimal Amount { get; set; }
    public int? CategoryId { get; set; }
    public string Currency { get; set; } = "TRY";
}