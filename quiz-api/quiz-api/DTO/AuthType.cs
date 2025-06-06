public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Username { get; set; }
}

public class VerifyEmailDto {
    public string Token { get; set; }
    public string Email { get; set; }
}