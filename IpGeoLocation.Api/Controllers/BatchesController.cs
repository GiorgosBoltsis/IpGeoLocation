using IpGeoLocation.Application.Batches.Dtos;
using IpGeoLocation.Application.Batches.Services;
using Microsoft.AspNetCore.Mvc;

namespace IpGeoLocation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BatchesController : ControllerBase
{
    private readonly BatchService _batchService;
    private readonly ILogger<BatchesController> _logger;

    public BatchesController(
        BatchService batchService,
        ILogger<BatchesController> logger)
    {
        _batchService = batchService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(BatchCreatedResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBatch(
        [FromBody] BatchCreateRequestDto request,
        CancellationToken cancellationToken)
    {
        if (request.IpAddresses is null || request.IpAddresses.Count == 0)
        {
            return BadRequest(new { error = "At least one IP address is required." });
        }

        try
        {
            var batch = await _batchService.CreateBatchAsync(request.IpAddresses, cancellationToken);

            var statusUrl = Url.Action(
                nameof(GetBatchStatus),
                "Batches",
                new { id = batch.Id },
                Request.Scheme) ?? $"/api/batches/{batch.Id}";

            var response = new BatchCreatedResponseDto(batch.Id, statusUrl);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid IPs in batch creation.");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BatchStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBatchStatus(Guid id, CancellationToken cancellationToken)
    {
        var status = await _batchService.GetBatchStatusAsync(id, cancellationToken);
        if (status is null)
        {
            return NotFound(new { error = "Batch not found." });
        }

        return Ok(status);
    }
}
