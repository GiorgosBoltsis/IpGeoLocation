using IpGeoLocation.Application.GeoIp.Dtos;

namespace IpGeoLocation.Application.GeoIp.Ports;

public interface IGeoIpLookupService
{
    Task<IpGeoLocationDto> LookupAsync(string ip, CancellationToken cancellationToken = default);
}
