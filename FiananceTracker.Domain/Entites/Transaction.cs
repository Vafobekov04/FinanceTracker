using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Entities;

public class Transaction
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid CategoryId { get; set; }

    public decimal Amount { get; set; }

    public TransactionType Type { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public string? Description { get; set; }

    public User User { get; set; } = null!;

    public Category Category { get; set; } = null!;
}