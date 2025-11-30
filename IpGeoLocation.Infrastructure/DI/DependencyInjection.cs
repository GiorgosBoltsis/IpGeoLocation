using IpGeoLocation.Application.Batches.Ports;
using IpGeoLocation.Application.GeoIp.Ports;
using IpGeoLocation.Application.Common;
using IpGeoLocation.Infrastructure.GeoIp;
using IpGeoLocation.Infrastructure.Persistence;
using IpGeoLocation.Infrastructure.Persistence.Repositories;
using IpGeoLocation.Infrastructure.Time;
using IpGeoLocation.Infrastructure.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IpGeoLocation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IBatchRepository, EfBatchRepository>();

        services.AddSingleton<ITimeProvider, SystemTimeProvider>();

        services.AddHttpClient<IGeoIpLookupService, FreeGeoIpLookupService>(client =>
        {
            client.BaseAddress = new Uri("https://freegeoip.app");
        });

        services.AddHostedService<BatchProcessorBackgroundService>();

        return services;
    }
}
