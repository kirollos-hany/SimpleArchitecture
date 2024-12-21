using LanguageExt;
using SimpleArchitecture.Authentication.Enums;
using SimpleArchitecture.Authentication.Types;
using SimpleArchitecture.Common.Response;

namespace SimpleArchitecture.Authentication.Interfaces;

public interface IExternalLoginIdTokenValidator
{
    Task<Either<FailureResponse, ExternalLoginUserData>> ValidateAsync(ExternalLoginProvider loginProvider, string idToken);
}