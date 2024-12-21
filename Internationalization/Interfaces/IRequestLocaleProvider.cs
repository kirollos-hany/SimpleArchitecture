using SimpleArchitecture.Internationalization.Enums;

namespace SimpleArchitecture.Internationalization.Interfaces;

public interface IRequestLocaleProvider
{
    Language GetLanguage();
}