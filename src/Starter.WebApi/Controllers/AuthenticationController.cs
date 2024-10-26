namespace Starter.WebApi.Controllers;

public class AuthenticationController(IJwtService jwtService) : StarterControllerBase
{
    private readonly IJwtService _jwtService = jwtService;

    [AllowAnonymous]
    [HttpPost("token")]
    public async Task<IActionResult> Token(HashedLoginRequest hashedLoginRequest)
    {
        LoginResponse result = await _jwtService.CreateToken(hashedLoginRequest);

        return Ok(result);
    }
}
