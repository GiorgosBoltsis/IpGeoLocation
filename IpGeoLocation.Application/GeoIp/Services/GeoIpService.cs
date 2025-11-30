using IpGeoLocation.Application.GeoIp.Dtos;
using IpGeoLocation.Application.GeoIp.Ports;
using IpGeoLocation.Domain.ValueObjects;

namespace IpGeoLocation.Application.GeoIp.Services;

public class GeoIpService
{
    private readonly IGeoIpLookupService _lookupService;

    public GeoIpService(IGeoIpLookupService lookupService)
    {
        _lookupService = lookupService;
    }

    public async Task<IpGeoLocationDto> LookupAsync(string ip, CancellationToken cancellationToken = default)
    {
        var ipAddress = IpAddress.Create(ip);

        return await _lookupService.LookupAsync(ipAddress.Value, cancellationToken);
    }
}
