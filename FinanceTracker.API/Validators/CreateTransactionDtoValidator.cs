using FluentValidation;
using FinanceTracker.API.DTOs;

namespace FinanceTracker.API.Validators;

public class CreateTransactionDtoValidator
    : AbstractValidator<CreateTransactionDto>
{
    public CreateTransactionDtoValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Категория обязательна.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Сумма должна быть больше 0.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Некорректный тип транзакции.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Описание не должно превышать 500 символов.");
    }
}