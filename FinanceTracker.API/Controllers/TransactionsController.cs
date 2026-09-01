using System.Security.Claims;
using FinanceTracker.API.DTOs;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FinanceTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;

    public TransactionsController(
        ApplicationDbContext context,
        IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    // POST: api/Transactions
    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionDto dto)
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

        var category = await _context.Categories
            .FirstOrDefaultAsync(c =>
                c.Id == dto.CategoryId &&
                c.UserId == userId);

        if (category == null)
        {
            return NotFound("Категория не найдена.");
        }

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = dto.CategoryId,
            Amount = dto.Amount,
            Type = dto.Type.ToString(),
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);

        await _context.SaveChangesAsync();

        // Очищаем кэш статистики после изменения данных
        await _cache.RemoveAsync($"statistics:{userId}");

        return Ok(new TransactionResponseDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Description = transaction.Description ?? string.Empty,
            Date = transaction.CreatedAt,
            Type = transaction.Type,
            CategoryId = transaction.CategoryId
        });
    }

    // GET: api/Transactions
    [HttpGet]
    public async Task<IActionResult> GetMyTransactions()
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

        var transactions = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new
            {
                t.Id,
                t.Amount,
                t.Type,
                t.Description,
                Date = t.CreatedAt,
                t.CategoryId,
                Category = t.Category == null
                    ? null
                    : new
                    {
                        t.Category.Id,
                        t.Category.Name,
                        t.Category.Type
                    }
            })
            .ToListAsync();

        return Ok(transactions);
    }

    // GET: api/Transactions/recent
    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentTransactions(int count = 5)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        count = Math.Clamp(count, 1, 50);

        var transactions = await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(count)
            .Select(t => new RecentTransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type,
                Description = t.Description,
                Date = t.CreatedAt,
                CategoryName = t.Category.Name
            })
            .ToListAsync();

        return Ok(transactions);
    }

    // PUT: api/Transactions/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateTransactionDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized();
        }

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t =>
                t.Id == id &&
                t.UserId == userId);

        if (transaction == null)
        {
            return NotFound("Транзакция не найдена.");
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c =>
                c.Id == dto.CategoryId &&
                c.UserId == userId);

        if (category == null)
        {
            return NotFound("Категория не найдена.");
        }

        transaction.CategoryId = dto.CategoryId;
        transaction.Amount = dto.Amount;
        transaction.Type = dto.Type.ToString();
        transaction.Description = dto.Description;

        await _context.SaveChangesAsync();

        // Очищаем кэш статистики после изменения данных
        await _cache.RemoveAsync($"statistics:{userId}");

        return Ok(new TransactionResponseDto
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Description = transaction.Description ?? string.Empty,
            Date = transaction.CreatedAt,
            Type = transaction.Type,
            CategoryId = transaction.CategoryId
        });
    }

    // DELETE: api/Transactions/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
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

        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t =>
                t.Id == id &&
                t.UserId == userId);

        if (transaction == null)
        {
            return NotFound("Транзакция не найдена.");
        }

        _context.Transactions.Remove(transaction);

        await _context.SaveChangesAsync();

        // Очищаем кэш статистики после изменения данных
        await _cache.RemoveAsync($"statistics:{userId}");

        return Ok("Транзакция удалена.");
    }
}

