using FinanceTracker.Domain.Enums;

namespace FinanceTracker.API.DTOs;

public class CreateCategoryDto
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public TransactionType Type { get; set; }
}