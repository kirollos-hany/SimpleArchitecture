using System.Security.Claims;
using SimpleArchitecture.Internationalization.Enums;

namespace SimpleArchitecture.Common.Utilities;

public static class HttpContextUtilities
{
    public static bool IsAuthenticated(this ClaimsPrincipal user) => user.Identity is not null && user.Identity.IsAuthenticated;

    public static Language GetValueFromAcceptLanguageHeader(this HttpRequest request)
    {
        var acceptLanguage = request.Headers["Accept-Language"];

        return Enum.GetValues<Language>().FirstOrDefault(language => language.ToString() == acceptLanguage);
    }
}