using System.Collections;
using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using SimpleArchitecture.Common.Utilities;
using SimpleArchitecture.TimeZones.Interfaces;

namespace SimpleArchitecture.TimeZones.Filters;

public class TimeZoneActionFilter : IEndpointFilter
{
    private readonly ITimeZoneProvider _timeZoneProvider;

    public TimeZoneActionFilter(ITimeZoneProvider timeZoneProvider)
    {
        _timeZoneProvider = timeZoneProvider;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var result = await next(context);

        if (result is not IValueHttpResult okResult)
            return result;

        if (okResult.Value is null)
        {
            return result;
        }

        ConvertDateTimeProperties(okResult.Value, _timeZoneProvider.GetRequestTimeZone());

        return result;
    }

    private void ConvertDateTimeProperties(object obj, TimeZoneInfo timeZone)
    {
        var properties = obj.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(DateTime))
            {
                var dateTime = ((DateTime)property.GetValue(obj)!).UtcToTimeZone(timeZone);
                property.SetValue(obj, dateTime);
            }
            else if (property.PropertyType == typeof(DateTime?))
            {
                var dateTime = ((DateTime?)property.GetValue(obj)).UtcToTimeZone(timeZone);
                if (dateTime.HasValue)
                {
                    property.SetValue(obj, dateTime);
                }
            }
            else if (IsCollectionType(property.PropertyType))
            {
                if (property.GetValue(obj) is not IEnumerable collection)
                    continue;

                foreach (var item in collection)
                {
                    ConvertDateTimeProperties(item, timeZone);
                }
            }
            else if (IsComplexType(property.PropertyType))
            {
                var nestedObject = property.GetValue(obj);
                if (nestedObject is not null)
                    ConvertDateTimeProperties(nestedObject, timeZone);
            }
        }
    }

    private static bool IsComplexType(Type type)
    {
        return !type.IsPrimitive && !type.IsEnum && type != typeof(string) && !type.IsValueType;
    }

    private static bool IsCollectionType(Type type)
    {
        return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
    }
}