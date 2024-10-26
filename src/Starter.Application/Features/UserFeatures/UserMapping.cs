using AutoMapper;
using Starter.Domain.Aggregates.UserAggregate;

namespace Starter.Application.Features.UserFeatures;

public class UserMapping : Profile
{
    public UserMapping()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();

        CreateMap<Address, UserAddressDto>();
        CreateMap<UserAddressDto, Address>();
    }
}