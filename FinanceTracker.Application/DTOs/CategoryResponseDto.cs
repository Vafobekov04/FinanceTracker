using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.DTOs;

public class CategoryResponseDto
{
    public Guid Id { get; set; }


public string Name { get; set; } = string.Empty;

    public CategoryType Type { get; set; }


}
