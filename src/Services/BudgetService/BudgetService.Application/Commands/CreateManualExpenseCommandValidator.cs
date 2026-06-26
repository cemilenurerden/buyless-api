using FluentValidation;

namespace BudgetService.Application.Commands;

public class CreateManualExpenseCommandValidator : AbstractValidator<CreateManualExpenseCommand>
{
    public CreateManualExpenseCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Tutar sıfırdan büyük olmalı.");

        RuleFor(x => x.OccurredAt)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)))
            .WithMessage("Harcama tarihi gelecekte bir tarih olamaz.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .MaximumLength(3);
    }
}
