using IpGeoLocation.Application.GeoIp.Dtos;
using IpGeoLocation.Application.GeoIp.Services;
using Microsoft.AspNetCore.Mvc;

namespace IpGeoLocation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeoIpController : ControllerBase
{
    private readonly GeoIpService _geoIpService;

    public GeoIpController(GeoIpService geoIpService)
    {
        _geoIpService = geoIpService;
    }

    [HttpGet("{ip}")]
    [ProducesResponseType(typeof(IpGeoLocationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(string ip, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _geoIpService.LookupAsync(ip, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
