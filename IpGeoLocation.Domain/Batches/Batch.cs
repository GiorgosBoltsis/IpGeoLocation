using IpGeoLocation.Domain.ValueObjects;

namespace IpGeoLocation.Domain.Batches;

public class Batch
{
    public Guid Id { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public BatchStatus Status { get; private set; }

    private readonly List<BatchItem> _items = new();
    public IReadOnlyCollection<BatchItem> Items => _items.AsReadOnly();

    public int TotalCount => _items.Count;
    public int CompletedCount => _items.Count(i => i.Status == BatchItemStatus.Completed);
    public int FailedCount => _items.Count(i => i.Status == BatchItemStatus.Failed);

    private Batch() { }

    private Batch(Guid id, DateTime createdAtUtc, IEnumerable<IpAddress> ipAddresses)
    {
        Id = id;
        CreatedAtUtc = createdAtUtc;
        Status = BatchStatus.Pending;

        foreach (var ip in ipAddresses)
        {
            _items.Add(new BatchItem(ip.Value, this));
        }
    }

    public static Batch Create(IEnumerable<IpAddress> ipAddresses, DateTime createdAtUtc)
    {
        var list = ipAddresses.ToList();
        if (!list.Any())
            throw new ArgumentException("Batch must contain at least one IP address.", nameof(ipAddresses));

        return new Batch(Guid.NewGuid(), createdAtUtc, list);
    }

    public void MarkInProgress()
    {
        if (Status == BatchStatus.Pending)
        {
            Status = BatchStatus.InProgress;
        }
    }

    public void TryComplete()
    {
        if (_items.All(i => i.Status == BatchItemStatus.Completed || i.Status == BatchItemStatus.Failed))
        {
            Status = FailedCount == 0 ? BatchStatus.Completed : BatchStatus.Failed;
            CompletedAtUtc = DateTime.UtcNow;
        }
    }
}
