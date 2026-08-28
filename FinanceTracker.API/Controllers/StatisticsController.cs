using System.Security.Claims;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly ApplicationDbContext _context;


public StatisticsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetStatistics(
        DateTime? from = null,
        DateTime? to = null)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var query = _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        if (from.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= from.Value);
        }

        if (to.HasValue)
        {
            var endDate = to.Value.Date.AddDays(1);
            query = query.Where(t => t.CreatedAt < endDate);
        }

        var income = await query
            .Where(t => t.Type == "Income")
            .SumAsync(t => (decimal?)t.Amount) ?? 0;

        var expense = await query
            .Where(t => t.Type == "Expense")
            .SumAsync(t => (decimal?)t.Amount) ?? 0;

        var transactionCount = await query.CountAsync();

        var categories = await query
            .GroupBy(t => new
            {
                t.CategoryId,
                t.Category.Name,
                t.Category.Type
            })
            .Select(g => new CategoryStatisticsDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                Type = g.Key.Type.ToString(),
                Amount = g.Sum(t => t.Amount)
            })
            .OrderByDescending(x => x.Amount)
            .ToListAsync();

        var result = new StatisticsResponseDto
        {
            Balance = income - expense,
            Income = income,
            Expense = expense,
            TransactionCount = transactionCount,
            Categories = categories
        };

        return Ok(result);
    }


}
