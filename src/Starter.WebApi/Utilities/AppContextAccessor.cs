using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Starter.WebApi.Utilities;

public class AppContextAccessor(IHttpContextAccessor httpContext) : IAppContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContext;

    public UserClaims UserClaims
    {
        get
        {
            string? sub = _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            _ = Guid.TryParse(sub, out Guid id);

            return new()
            {
                UserId = id
            };
        }
    }
}
