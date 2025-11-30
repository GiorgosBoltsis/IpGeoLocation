using IpGeoLocation.Domain.Batches;

namespace IpGeoLocation.Application.Batches.Ports;

public interface IBatchRepository
{
    Task<Batch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Batch batch, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
