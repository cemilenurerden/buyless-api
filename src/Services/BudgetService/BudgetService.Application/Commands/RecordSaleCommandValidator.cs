using FluentValidation;

namespace BudgetService.Application.Commands;

public class RecordSaleCommandValidator : AbstractValidator<RecordSaleCommand>
{
    public RecordSaleCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Satış tutarı sıfırdan büyük olmalı.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .MaximumLength(3);
    }
}