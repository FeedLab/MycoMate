using Microsoft.EntityFrameworkCore;
using MycoMate.Api.Data;
using MycoMate.Api.Models;

namespace MycoMate.Api.Repositories;

public class VitaminRepository(MycoMateDbContext db) : IVitaminRepository
{
    public async Task<IEnumerable<Vitamin>> GetAllAsync(CancellationToken ct = default)
        => await db.Vitamins.OrderBy(v => v.Name).ToListAsync(ct);

    public async Task<Vitamin?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Vitamins.FindAsync([id], ct);
}
