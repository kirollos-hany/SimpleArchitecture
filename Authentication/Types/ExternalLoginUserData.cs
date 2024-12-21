namespace SimpleArchitecture.Authentication.Types;

public record ExternalLoginUserData(string Email, string Subject, string? UserName, string? ProfilePicture);