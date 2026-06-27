using MediatR;

namespace BudgetService.Application.Commands;

public class RecordDonationCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public Guid ItemId { get; set; }
    public int? CategoryId { get; set; }
}
