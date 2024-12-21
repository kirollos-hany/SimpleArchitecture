using System.Collections;
using System.Reflection;
using SimpleArchitecture.IO.Attributes;
using SimpleArchitecture.IO.Interfaces;

namespace SimpleArchitecture.IO.Filters;

public class RequiresBaseUrlFilter : IEndpointFilter
{
    private readonly IFileManager _fileManager;

    public RequiresBaseUrlFilter(IFileManager fileManager)
    {
        _fileManager = fileManager;
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

        await SetBaseUrl(okResult.Value);

        return result;
    }

    private async Task SetBaseUrl(object obj)
    {
        var properties = obj.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(string) && property.IsDefined(typeof(RequiresBaseUrl)))
            {
                var value = (string)property.GetValue(obj)!;

                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                var valueWithBaseUrl = await _fileManager.GetUrlAsync(value);

                property.SetValue(obj, valueWithBaseUrl);

                continue;
            }

            if (IsCollectionType(property.PropertyType))
            {
                if (property.GetValue(obj) is not IEnumerable collection)
                    continue;

                var tasks = new List<Task>();

                foreach (var item in collection)
                {
                    tasks.Add(SetBaseUrl(item));
                }

                await Task.WhenAll(tasks);

                continue;
            }

            if (IsComplexType(property.PropertyType))
            {
                var nestedObject = property.GetValue(obj);

                if (nestedObject is not null)
                    await SetBaseUrl(nestedObject);
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