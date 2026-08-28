using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.DTOs;

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;


public CategoryType Type { get; set; }
}
