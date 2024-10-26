namespace Starter.WebApi.Controllers;

public class UserController(IUserService userService) : StarterControllerBase
{
    private readonly IUserService _userService = userService;

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateUser(UserDto userDto)
    {
        UserDto resultDto = await _userService.CreateUser(userDto);

        return Ok(resultDto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ReadUser(Guid id)
    {
        UserDto resultDto = await _userService.ReadUser(id);

        return Ok(resultDto);
    }
}
