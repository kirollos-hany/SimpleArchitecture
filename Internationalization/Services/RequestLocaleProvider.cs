using SimpleArchitecture.Common.Utilities;
using SimpleArchitecture.Internationalization.Enums;
using SimpleArchitecture.Internationalization.Interfaces;

namespace SimpleArchitecture.Internationalization.Services;

public class RequestLocaleProvider : IRequestLocaleProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestLocaleProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Language GetLanguage()
    {
        var context = _httpContextAccessor.HttpContext;

        if (context is null)
        {
            return Language.Ar;
        }

        var language = context.Request.GetValueFromAcceptLanguageHeader();

        return language;
    }
}