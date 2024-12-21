namespace SimpleArchitecture.TimeZones.Interfaces;

public interface ITimeZoneProvider
{
    TimeZoneInfo GetRequestTimeZone();
}