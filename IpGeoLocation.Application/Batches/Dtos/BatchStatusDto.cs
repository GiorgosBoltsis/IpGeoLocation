namespace IpGeoLocation.Application.Batches.Dtos;

public record BatchStatusDto(
    Guid BatchId,
    int TotalCount,
    int CompletedCount,
    int FailedCount,
    string Status,
    double ProgressPercentage,
    string? EstimatedCompletion);
