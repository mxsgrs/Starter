using Starter.Domain.Authentication;

namespace Starter.Application.Features.AuthenticationFeatures;

public interface IJwtService
{
    Task<LoginResponse> CreateToken(HashedLoginRequest hashedLoginRequest);
}