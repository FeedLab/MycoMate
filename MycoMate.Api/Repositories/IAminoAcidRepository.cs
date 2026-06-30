using MycoMate.Api.Models;

namespace MycoMate.Api.Repositories;

public interface IAminoAcidRepository
{
    Task<IEnumerable<AminoAcid>> GetAllAsync(CancellationToken ct = default);
    Task<AminoAcid?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
