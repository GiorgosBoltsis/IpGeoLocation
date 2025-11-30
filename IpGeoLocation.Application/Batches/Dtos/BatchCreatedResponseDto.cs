namespace IpGeoLocation.Application.Batches.Dtos;

public record BatchCreatedResponseDto(
    Guid BatchId,
    string StatusUrl);
