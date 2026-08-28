using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.DTOs;

public class UpdateTransactionDto
{
    public Guid CategoryId { get; set; }


public decimal Amount { get; set; }

    public TransactionType Type { get; set; }

    public string? Description { get; set; }


}
