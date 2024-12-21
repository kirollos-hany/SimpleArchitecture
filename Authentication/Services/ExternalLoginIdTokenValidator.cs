using Google.Apis.Auth;
using LanguageExt;
using SimpleArchitecture.Authentication.Configurations;
using SimpleArchitecture.Authentication.Enums;
using SimpleArchitecture.Authentication.Interfaces;
using SimpleArchitecture.Authentication.Types;
using SimpleArchitecture.Common.Response;

namespace SimpleArchitecture.Authentication.Services;

public class ExternalLoginIdTokenValidator : IExternalLoginIdTokenValidator
{
    private readonly GoogleConfig _googleConfig;

    private readonly ILogger<ExternalLoginIdTokenValidator> _logger;

    private readonly IReadOnlyDictionary<ExternalLoginProvider, Func<string, Task<Either<FailureResponse, ExternalLoginUserData>>>> _tokenValidationMap;

    public ExternalLoginIdTokenValidator(GoogleConfig googleConfig, ILogger<ExternalLoginIdTokenValidator> logger)
    {
        _googleConfig = googleConfig;
        _logger = logger;

        _tokenValidationMap = new Dictionary<ExternalLoginProvider, Func<string, Task<Either<FailureResponse, ExternalLoginUserData>>>>
        {
            { ExternalLoginProvider.Google, ValidateGoogleIdToken}
        };
    }

    public Task<Either<FailureResponse, ExternalLoginUserData>> ValidateAsync(ExternalLoginProvider loginProvider, string idToken) => _tokenValidationMap[loginProvider](idToken);
    
    private async Task<Either<FailureResponse, ExternalLoginUserData>> ValidateGoogleIdToken(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { _googleConfig.ClientId }
            };
            
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new ExternalLoginUserData(payload.Email, payload.Subject, payload.Name, payload.Picture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Validating id token caused the following exception");

            var response = new ValidationFailureResponse(new Dictionary<string, string> { { nameof(idToken), "Id token is invalid" } });

            return FailureResponse<ValidationFailureResponse>.ValidationFailure(response);
        }
    }
}