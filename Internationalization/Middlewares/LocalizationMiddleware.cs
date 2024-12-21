using System.Globalization;
using SimpleArchitecture.Common.Utilities;

namespace SimpleArchitecture.Internationalization.Middlewares;

public class LocalizationMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var language = context.Request.GetValueFromAcceptLanguageHeader();

        var locale = language.ToLocale();

        var culture = new CultureInfo(locale);
        
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        context.Response.Headers["Content-Language"] = language.ToString();
        context.Response.Headers["Accept-Language"] = language.ToString();
        
        return next(context);
    }
}