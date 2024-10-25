﻿using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Starter.Application.AuthenticationFeatures;

public class JwtService(ILogger<JwtService> logger, IConfiguration configuration,
    IUserRepository userService) : IJwtService
{
    private readonly ILogger<JwtService> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly IUserRepository _userService = userService;

    public async Task<LoginResponse> CreateToken(HashedLoginRequest hashedLoginRequest)
    {
        _logger.LogDebug("Hashed login request is {HashedLoginRequest}", hashedLoginRequest);

        Guid result = await ValidateUser(hashedLoginRequest);

        JsonWebTokenParameters jwtParameters = _configuration
            .GetRequiredSection("JsonWebTokenParameters")
            .Get<JsonWebTokenParameters>()
                ?? throw new Exception("JWT settings are not configured");

        byte[] encodedKey = Encoding.ASCII.GetBytes(jwtParameters.Key);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, result.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, hashedLoginRequest.EmailAddress)
            ]),
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = jwtParameters.Issuer,
            Audience = jwtParameters.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(encodedKey),
                SecurityAlgorithms.HmacSha512Signature)
        };

        JsonWebTokenHandler handler = new()
        {
            SetDefaultTimesOnTokenCreation = false
        };

        string accessToken = handler.CreateToken(tokenDescriptor);

        LoginResponse loginResponse = new()
        {
            AccessToken = accessToken
        };

        return loginResponse;
    }

    /// <summary>
    /// Verify the user exists in the database
    /// </summary>
    private async Task<Guid> ValidateUser(HashedLoginRequest hashedLoginRequest)
    {
        User result = await _userService.ReadUser(hashedLoginRequest.EmailAddress,
            hashedLoginRequest.HashedPassword);

        return result.Id;
    }
}
