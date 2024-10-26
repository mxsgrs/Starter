namespace Starter.Domain.Authentication;

public class HashedLoginRequest
{
    public string EmailAddress { get; set; } = "";
    public string HashedPassword { get; set; } = "";
}

public class PlainLoginRequest
{
    public string EmailAddress { get; set; } = "";
    public string PlainPassword { get; set; } = "";
    public bool RememberMe { get; set; } = false;
}

public class LoginResponse
{
    public string AccessToken { get; set; } = "";
}
