using IpGeoLocation.Application.Batches.Ports;
using IpGeoLocation.Domain.Batches;
using Microsoft.EntityFrameworkCore;

namespace IpGeoLocation.Infrastructure.Persistence.Repositories;

public class EfBatchRepository : IBatchRepository
{
    private readonly AppDbContext _context;

    public EfBatchRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Batch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Batches
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task AddAsync(Batch batch, CancellationToken cancellationToken = default)
    {
        await _context.Batches.AddAsync(batch, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
