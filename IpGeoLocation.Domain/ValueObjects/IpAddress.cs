using System.Net;

namespace IpGeoLocation.Domain.ValueObjects;

public sealed class IpAddress
{
    public string Value { get; }

    private IpAddress(string value)
    {
        Value = value;
    }

    public static IpAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("IP address cannot be empty.", nameof(value));

        if (!IPAddress.TryParse(value, out _))
            throw new ArgumentException("Invalid IP address format.", nameof(value));

        return new IpAddress(value);
    }

    public override string ToString() => Value;
}
