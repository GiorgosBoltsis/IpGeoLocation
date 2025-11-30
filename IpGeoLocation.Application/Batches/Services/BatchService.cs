using IpGeoLocation.Application.Batches.Dtos;
using IpGeoLocation.Application.Batches.Ports;
using IpGeoLocation.Application.Common;
using IpGeoLocation.Domain.Batches;
using IpGeoLocation.Domain.ValueObjects;

namespace IpGeoLocation.Application.Batches.Services;

public class BatchService
{
    private readonly IBatchRepository _batchRepository;
    private readonly ITimeProvider _timeProvider;

    public BatchService(
        IBatchRepository batchRepository,
        ITimeProvider timeProvider)
    {
        _batchRepository = batchRepository;
        _timeProvider = timeProvider;
    }

    public async Task<Batch> CreateBatchAsync(
        IReadOnlyCollection<string> ips,
        CancellationToken cancellationToken = default)
    {
        var ipValueObjects = ips
            .Select(IpAddress.Create)
            .ToList();

        var batch = Batch.Create(ipValueObjects, _timeProvider.UtcNow);

        await _batchRepository.AddAsync(batch, cancellationToken);
        await _batchRepository.SaveChangesAsync(cancellationToken);

        return batch;
    }

    public async Task<BatchStatusDto?> GetBatchStatusAsync(
    Guid batchId,
    CancellationToken cancellationToken = default)
    {
        var batch = await _batchRepository.GetByIdAsync(batchId, cancellationToken);
        if (batch is null)
            return null;

        var total = batch.TotalCount;
        var completed = batch.CompletedCount;
        var failed = batch.FailedCount;
        var processed = completed + failed;
        var now = _timeProvider.UtcNow;

        var progress = total == 0
            ? 0
            : (double)completed / total * 100.0;

        string? estimatedCompletion = null;

        if (processed > 0 && processed < total)
        {
            var elapsed = now - batch.CreatedAtUtc;
            if (elapsed.TotalSeconds > 0)
            {
                var avgPerItemSeconds = elapsed.TotalSeconds / processed;
                var remaining = total - processed;
                var remainingSeconds = avgPerItemSeconds * remaining;
                var eta = now.AddSeconds(remainingSeconds);
                estimatedCompletion = eta.ToString("O");
            }
        }
        else if (batch.CompletedAtUtc.HasValue)
        {
            estimatedCompletion = batch.CompletedAtUtc.Value.ToString("O");
        }

        return new BatchStatusDto(
            batch.Id,
            total,
            completed,
            failed,
            batch.Status.ToString(),
            progress,
            estimatedCompletion);
    }

}
