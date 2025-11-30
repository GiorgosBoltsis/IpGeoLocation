namespace IpGeoLocation.Application.Batches.Dtos;

public record BatchCreateRequestDto(
    IReadOnlyCollection<string> IpAddresses);
