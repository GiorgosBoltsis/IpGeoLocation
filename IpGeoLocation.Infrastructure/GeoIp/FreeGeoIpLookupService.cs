using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using IpGeoLocation.Application.GeoIp.Dtos;
using IpGeoLocation.Application.GeoIp.Ports;

namespace IpGeoLocation.Infrastructure.GeoIp;

public class FreeGeoIpLookupService : IGeoIpLookupService
{
    private readonly HttpClient _httpClient;

    public FreeGeoIpLookupService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IpGeoLocationDto> LookupAsync(string ip, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"/json/{ip}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var data = JsonSerializer.Deserialize<FreeGeoIpResponse>(rawJson, options);

        if (data is null)
        {
            throw new InvalidOperationException("Failed to deserialize FreeGeoIP response.");
        }

        return new IpGeoLocationDto(
            data.Ip,
            data.CountryCode,
            data.CountryName,
            data.TimeZone,
            data.Latitude,
            data.Longitude);
    }

    private sealed class FreeGeoIpResponse
    {
        [JsonPropertyName("ip")]
        public string Ip { get; set; } = default!;

        [JsonPropertyName("country_code")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("country_name")]
        public string? CountryName { get; set; }

        [JsonPropertyName("time_zone")]
        public string? TimeZone { get; set; }

        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }
    }
}
