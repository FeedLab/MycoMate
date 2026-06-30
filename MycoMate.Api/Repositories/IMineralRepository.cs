using MycoMate.Api.Models;

namespace MycoMate.Api.Repositories;

public interface IMineralRepository
{
    Task<IEnumerable<Mineral>> GetAllAsync(CancellationToken ct = default);
    Task<Mineral?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
