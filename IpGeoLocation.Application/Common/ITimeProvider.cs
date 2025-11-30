namespace IpGeoLocation.Application.Common;

public interface ITimeProvider
{
    DateTime UtcNow { get; }
}
