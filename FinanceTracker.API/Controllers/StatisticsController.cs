using System.Security.Claims;
using System.Text.Json;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

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

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        // Уникальный ключ кэша для пользователя и периода
        var cacheKey = $"statistics:{userId}:{from}:{to}";

        // 1. Проверяем Redis
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (cachedData != null)
        {
            var cachedResult =
                JsonSerializer.Deserialize<StatisticsResponseDto>(cachedData);

            if (cachedResult != null)
            {
                return Ok(cachedResult);
            }
        }

        // 2. Если в Redis ничего нет — идём в PostgreSQL
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

        // 3. Сохраняем результат в Redis
        var json = JsonSerializer.Serialize(result);

        await _cache.SetStringAsync(
            cacheKey,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

        return Ok(result);
    }
}