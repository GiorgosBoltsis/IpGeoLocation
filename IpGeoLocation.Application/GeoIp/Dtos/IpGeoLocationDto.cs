namespace IpGeoLocation.Application.GeoIp.Dtos;

public record IpGeoLocationDto(
    string Ip,
    string CountryCode,
    string CountryName,
    string TimeZone,
    double? Latitude,
    double? Longitude);
