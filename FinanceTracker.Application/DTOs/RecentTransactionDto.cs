namespace FinanceTracker.Application.DTOs;

public class RecentTransactionDto
{
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public string Type { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime Date { get; set; }

    public string CategoryName { get; set; } = string.Empty;
}