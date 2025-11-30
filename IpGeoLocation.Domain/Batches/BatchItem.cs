using IpGeoLocation.Domain.GeoLocations;

namespace IpGeoLocation.Domain.Batches;

public class BatchItem
{
    public Guid Id { get; private set; }
    public Guid BatchId { get; private set; }
    public Batch Batch { get; private set; } = default!;
    public string Ip { get; private set; } = default!;
    public BatchItemStatus Status { get; private set; }
    public string? ErrorMessage { get; private set; }

    public string? CountryCode { get; private set; }
    public string? CountryName { get; private set; }
    public string? TimeZone { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }

    private BatchItem() { }

    public BatchItem(string ip, Batch batch)
    {
        Id = Guid.NewGuid();
        Ip = ip;
        Batch = batch;
        BatchId = batch.Id;
        Status = BatchItemStatus.Pending;
    }

    public void MarkInProgress()
    {
        if (Status == BatchItemStatus.Pending)
        {
            Status = BatchItemStatus.InProgress;
        }
    }

    public void MarkCompleted(IpGeoLocationResult result)
    {
        Status = BatchItemStatus.Completed;
        Ip = result.Ip;
        CountryCode = result.CountryCode;
        CountryName = result.CountryName;
        TimeZone = result.TimeZone;
        Latitude = result.Latitude;
        Longitude = result.Longitude;
        ErrorMessage = null;
    }

    public void MarkFailed(string error)
    {
        Status = BatchItemStatus.Failed;
        ErrorMessage = error;
    }
}
