using Microsoft.EntityFrameworkCore;
using MycoMate.Api.Data;
using MycoMate.Api.Models;

namespace MycoMate.Api.Repositories;

public class AminoAcidRepository(MycoMateDbContext db) : IAminoAcidRepository
{
    public async Task<IEnumerable<AminoAcid>> GetAllAsync(CancellationToken ct = default)
        => await db.AminoAcids.OrderBy(a => a.Name).ToListAsync(ct);

    public async Task<AminoAcid?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.AminoAcids.FindAsync([id], ct);
}
