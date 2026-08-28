namespace FinanceTracker.Application.DTOs;

public class StatisticsResponseDto
{
    public decimal Balance { get; set; }


public decimal Income { get; set; }

    public decimal Expense { get; set; }

    public int TransactionCount { get; set; }

    public List<CategoryStatisticsDto> Categories { get; set; } = new();


}

public class CategoryStatisticsDto
{
    public Guid CategoryId { get; set; }


public string CategoryName { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public decimal Amount { get; set; }


}
