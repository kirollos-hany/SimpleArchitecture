namespace SimpleArchitecture.Common.Utilities;

public static class DateUtilities
{
    public static DateTime MonthStart()
    {
        var now = DateTime.UtcNow;

        return new DateTime(year: now.Year, month: now.Month, day: 1);
    }

    public static DateTime TodayUtcToTimeZone(TimeZoneInfo timezoneInfo)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezoneInfo);
    }

    public static DateTime UtcToTimeZone(this DateTime dateTime, TimeZoneInfo timezoneInfo)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timezoneInfo);
    }
    
    public static DateTime? UtcToTimeZone(this DateTime? dateTime, TimeZoneInfo timezoneInfo)
    {
        if (dateTime is null)
        {
            return null;
        }
        
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime.Value, timezoneInfo);
    }
}