using SimpleArchitecture.Internationalization.Enums;

namespace SimpleArchitecture.Common.Utilities;

public static class LanguageUtilities
{
    public static string ToLocale(this Language language)
    {
        return language switch
        {
            Language.Ar => "ar-EG",
            Language.En => "en-US",
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
    }
}