using System.Security.Claims;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController : ControllerBase
{

    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;

    public StatisticsController(
        ApplicationDbContext context,
        IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
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
    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlyStatistics(
        int? year = null)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var selectedYear = year ?? DateTime.UtcNow.Year;

        var transactions = await _context.Transactions
            .AsNoTracking()
            .Where(t =>
                t.UserId == userId &&
                t.CreatedAt.Year == selectedYear)
            .GroupBy(t => t.CreatedAt.Month)
            .Select(g => new
            {
                Month = g.Key,
                Income = g
                    .Where(t => t.Type == "Income")
                    .Sum(t => (decimal?)t.Amount) ?? 0,

                Expense = g
                    .Where(t => t.Type == "Expense")
                    .Sum(t => (decimal?)t.Amount) ?? 0
            })
            .OrderBy(x => x.Month)
            .ToListAsync();

        var result = transactions.Select(x => new MonthlyStatisticsDto
        {
            Year = selectedYear,
            Month = x.Month,
            Income = x.Income,
            Expense = x.Expense
        });

        return Ok(result);
    }
}


