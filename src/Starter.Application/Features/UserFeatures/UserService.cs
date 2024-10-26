using AutoMapper;
using Microsoft.Extensions.Logging;
using Starter.Domain.Aggregates.UserAggregate;

namespace Starter.Application.Features.UserFeatures;

public class UserService(ILogger<UserService> logger, IMapper mapper,
    IUserRepository userRepository) : IUserService
{
    private readonly ILogger<UserService> _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserDto> CreateUser(UserDto userDto)
    {
        _logger.LogInformation("Mapping UserDto to User");

        User user = _mapper.Map<User>(userDto);
        User createdUser = await _userRepository.CreateUser(user);

        UserDto result = _mapper.Map<UserDto>(createdUser);

        return result;
    }

    public async Task<UserDto> ReadUser(Guid id)
    {
        _logger.LogInformation("Reading user with id {Id}", id);

        User user = await _userRepository.ReadUser(id);

        UserDto result = _mapper.Map<UserDto>(user);

        return result;
    }

    public async Task<UserDto> ReadUser(string emailAddress, string hashedPassword)
    {
        _logger.LogInformation("Reading user with email {Email}", emailAddress);

        User user = await _userRepository.ReadUser(emailAddress, hashedPassword);

        UserDto result = _mapper.Map<UserDto>(user);

        return result;
    }

    public async Task<UserDto> UpdateUser(Guid id, UserDto userDto)
    {
        _logger.LogInformation("Updating user with id {Id}", id);

        User user = _mapper.Map<User>(userDto);

        User updatedUser = await _userRepository.UpdateUser(id, user);

        UserDto result = _mapper.Map<UserDto>(updatedUser);

        return result;
    }
}
