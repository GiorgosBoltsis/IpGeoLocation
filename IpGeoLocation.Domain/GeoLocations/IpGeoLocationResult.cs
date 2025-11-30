namespace IpGeoLocation.Domain.GeoLocations;

public class IpGeoLocationResult
{
    public string Ip { get; private set; } = default!;
    public string? CountryCode { get; private set; }
    public string? CountryName { get; private set; }
    public string? TimeZone { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }

    private IpGeoLocationResult() { }

    public IpGeoLocationResult(
        string ip,
        string? countryCode,
        string? countryName,
        string? timeZone,
        double? latitude,
        double? longitude)
    {
        Ip = ip;
        CountryCode = countryCode;
        CountryName = countryName;
        TimeZone = timeZone;
        Latitude = latitude;
        Longitude = longitude;
    }
}
