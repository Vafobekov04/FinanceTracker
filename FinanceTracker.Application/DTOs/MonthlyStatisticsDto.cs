namespace FinanceTracker.Application.DTOs;

public class MonthlyStatisticsDto
{
    public int Year { get; set; }

    public int Month { get; set; }

    public decimal Income { get; set; }

    public decimal Expense { get; set; }
}