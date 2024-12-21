using System.Text.RegularExpressions;

namespace SimpleArchitecture.Common;

public static class Constants
{
    public static class MemoryCacheIdentifiers
    {
        public const string Separator = "__";

        public const string SendOtpRateLimit = "SendOtp";

        public const string OtpRepository = "Otp";

        public const string ProceedToTwoFactorAuthTokenRepository = "ProceedTo2FA";
    }

    public static class ValidationMessages
    {
        public static class NotFoundMessages
        {
            public const string UserNotFound = "Account couldn't be found.";
        }

        public static class EmailOrPhoneMessages
        {
            public const string InvalidEmailOrPhone = "Email or phone is invalid.";
        }

        public static class AuthenticationMessages
        {
            public const string TwoFactorDisabled =
                "Two factor authentication is disabled, please enable first then try again.";

            public const string ProceedTokenNotSet = "Proceed token isn't set, please login using credentials first";

            public const string ProceedTokenExpired = "Proceed token has expired, please login again";

            public const string ProceedTokenInvalid = "Proceed token is invalid";

            public const string TwoFactorAuthCodeInvalid = "Authenticator code is invalid";

            public const string RecoveryCodeInvalid = "Recovery code is invalid";
        }

        public static class EmailValidation
        {
            public const string Required = "Email is required.";

            public const string Format = "Email is invalid.";

            public const string NotRegistered = "Email provided is not registered.";
        }

        public static class PasswordValidation
        {
            public const string Required = "Password is required.";

            public const string Format =
                "Password must be at least 8 characters, contains a symbol, uppercase and a number";

            public const string Incorrect = "Password provided is incorrect.";

            public const string PasswordsNotMatch = "Password and Confirm Password doesn't match";
        }

        public static class PhoneNumberValidation
        {
            public const string Required = "Phone number is required.";

            public const string Format =
                "Phone number must be of format: +(countryCode)(phoneNumber)";

            public const string NotRegistered = "Phone number provided is not registered.";
        }

        public static class RefreshTokenValidation
        {
            public const string Invalid = "Refresh token is invalid";
        }

        public static class OtpVerificationValidation
        {
            public const string OtpInvalid = "Otp is invalid.";

            public const string OtpExpired = "Otp has expired.";
        }
    }

    public static class ValidationRegex
    {
        // regex for minimum length of 8, a symbol, uppercase and a number
        public static readonly Regex Password = new("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$");

        // regex to match phone number with format: +(countryCode)(phoneNumber) example: +201201523156
        public static readonly Regex PhoneNumber = new("^\\+\\d{1,4}\\d{9,15}$");
    }

    public static class OtpMessages
    {
        public const string EmailSentMessage = "Please check your inbox for an otp sent to verify your account.";

        public const string PhoneNumberSentMessage = "Please check your phone for an otp sent to verify your account.";

        public const string AccountVerificationSubject = "MyTemplate Account Verification";

        public const string PasswordResetSubject = "MyTemplate Password Reset";

        public static string GetOtpBodyMessage(string code) =>
            $"Please enter the following code to verify your account: {code}";
    }

    public static class ResponseMessages
    {
        public const string InternalServerErrorMessage = "Something went wrong. we're working on fixing it asap!";

        public const string PasswordChangedSuccessfully = "Password changed successfully";

        public static string AccountToggleMessage(bool active) =>
            active ? "Account activated successfully" : "Account de-activated successfully";
    }

    public static class SimpleArchitectureAdmin
    {
        public const string Email = "admin@simplearchitecture.com";

        public const string Password = "Admin#123";

        public const string UserName = "SimpleArchitectureAdmin";

        public const string DisplayName = "SimpleArchitecture Administrator";
    }

    public static class SystemAdministrator
    {
        public const string Email = "supervisor@simplearchitecture.com";

        public const string Password = "Supervisor#123";

        public const string UserName = "SimpleArchitectureSupervisor";

        public const string DisplayName = "SimpleArchitecture Supervisor";
    }

    public static class QueryExpressionBuilder
    {
        public static class ComparisonMethods
        {
            public const string LessThan = "lt";

            public const string LessThanOrEqual = "lte";

            public const string GreaterThan = "gt";

            public const string GreaterThanOrEqual = "gte";
        }
    }

    public static class QueryParser
    {
        public static class ParserTokens
        {
            public const char NestedPropertyIndicator = '_';

            public const string MethodIndicator = "__";

            public const char QueryStringSeparator = '&';

            public const char PropertyValueSeparator = '=';
        }
    }

    public static class AuthenticationSchemes
    {
        public const string JwtOrCookies = "JWT_OR_COOKIES";
    }
}