using MycoMate.Api.Models;

namespace MycoMate.Api.Repositories;

public interface IVitaminRepository
{
    Task<IEnumerable<Vitamin>> GetAllAsync(CancellationToken ct = default);
    Task<Vitamin?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
