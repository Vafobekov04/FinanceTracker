using FinanceTracker.Domain.Entities;

namespace FinanceTracker.API.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}
