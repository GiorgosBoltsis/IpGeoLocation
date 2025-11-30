using IpGeoLocation.Application.Common;

namespace IpGeoLocation.Infrastructure.Time;

public class SystemTimeProvider : ITimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
