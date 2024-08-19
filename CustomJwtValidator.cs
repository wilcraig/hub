using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

public class CustomJwtValidator : TokenHandler
{
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public CustomJwtValidator()
    {
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public bool CanReadToken(string securityToken)
    {
        return _tokenHandler.CanReadToken(securityToken);
    }

    public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
    {
        // Custom validation logic here
        var principal = _tokenHandler.ValidateToken(securityToken, validationParameters, out validatedToken);

        // Add custom validation checks if needed
        // For example, log the token details
        Console.WriteLine($"Token: {securityToken}");
        Console.WriteLine($"Validated Token: {validatedToken}");

        return principal;
    }

    public bool CanValidateToken => _tokenHandler.CanValidateToken;

    public int MaximumTokenSizeInBytes
    {
        get => _tokenHandler.MaximumTokenSizeInBytes;
        set => _tokenHandler.MaximumTokenSizeInBytes = value;
    }
}
