using Microsoft.EntityFrameworkCore;
using MycoMate.Api.Data;
using MycoMate.Api.Models;

namespace MycoMate.Api.Repositories;

public class MineralRepository(MycoMateDbContext db) : IMineralRepository
{
    public async Task<IEnumerable<Mineral>> GetAllAsync(CancellationToken ct = default)
        => await db.Minerals.OrderBy(m => m.Name).ToListAsync(ct);

    public async Task<Mineral?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Minerals.FindAsync([id], ct);
}
