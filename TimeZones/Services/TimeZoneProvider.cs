using SimpleArchitecture.TimeZones.Configurations;
using SimpleArchitecture.TimeZones.Interfaces;

namespace SimpleArchitecture.TimeZones.Services;

public class TimeZoneProvider : ITimeZoneProvider
{
    private readonly HttpContext _httpContext;

    private readonly TimeZoneConfiguration _configuration;

    public TimeZoneProvider(IHttpContextAccessor contextAccessor, TimeZoneConfiguration configuration)
    {
        _configuration = configuration;
        _httpContext = contextAccessor.HttpContext!;
    }

    public TimeZoneInfo GetRequestTimeZone()
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_configuration.Default);

        var headerValue = _httpContext.Request.Headers[_configuration.HeaderKey];

        var value = headerValue.ToString();

        if (!string.IsNullOrEmpty(value))
        {
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(value);
        }

        return timeZone;
    }
}