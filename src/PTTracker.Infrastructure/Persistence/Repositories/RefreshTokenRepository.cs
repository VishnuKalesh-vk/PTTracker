using Microsoft.EntityFrameworkCore;
using PTTracker.Application.Common.Interfaces;
using PTTracker.Domain.Entities;

namespace PTTracker.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(AppDbContext dbContext) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        => await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        => await dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash, cancellationToken);
}
