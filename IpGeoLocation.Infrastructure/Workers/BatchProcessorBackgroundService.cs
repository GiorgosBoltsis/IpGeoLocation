using IpGeoLocation.Application.GeoIp.Ports;
using IpGeoLocation.Domain.Batches;
using IpGeoLocation.Domain.GeoLocations;
using IpGeoLocation.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IpGeoLocation.Infrastructure.Workers;

public class BatchProcessorBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BatchProcessorBackgroundService> _logger;

    public BatchProcessorBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<BatchProcessorBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Batch processor background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var geoService = scope.ServiceProvider.GetRequiredService<IGeoIpLookupService>();

                var item = await db.BatchItems
                    .Include(i => i.Batch)
                    .Where(i => i.Status == BatchItemStatus.Pending)
                    .OrderBy(i => i.Id)
                    .FirstOrDefaultAsync(stoppingToken);

                if (item == null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                    continue;
                }

                item.MarkInProgress();
                item.Batch.MarkInProgress();
                await db.SaveChangesAsync(stoppingToken);

                try
                {
                    var dto = await geoService.LookupAsync(item.Ip, stoppingToken);

                    var result = new IpGeoLocationResult(
                        dto.Ip,
                        dto.CountryCode,
                        dto.CountryName,
                        dto.TimeZone,
                        dto.Latitude,
                        dto.Longitude);

                    item.MarkCompleted(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing IP {Ip}", item.Ip);
                    item.MarkFailed(ex.Message);
                }

                item.Batch.TryComplete();
                await db.SaveChangesAsync(stoppingToken);
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in batch processor loop.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("Batch processor background service stopping.");
    }
}
