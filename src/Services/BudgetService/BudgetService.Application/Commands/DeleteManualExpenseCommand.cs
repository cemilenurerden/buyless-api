using MediatR;

namespace BudgetService.Application.Commands;

public class DeleteManualExpenseCommand : IRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}