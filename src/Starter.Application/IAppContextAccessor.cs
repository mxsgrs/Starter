using Starter.Domain.Authentication;

namespace Starter.Application;

public interface IAppContextAccessor
{
    UserClaims UserClaims { get; }
}