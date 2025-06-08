using quiz_api.Service;

public interface IAuthService
{
    Task<UserDto> LoginAsync(LoginDto login);
    Task RegisterAsync(RegisterDto registerData);
    Task VerifyEmailAsync(VerifyEmailDto verifyData);
}

public class AuthService(
    IUserRepository userRepository,
    IEmailService emailService,
    IQuizRepository quizRepository,
    EnvVars envVars)
    : IAuthService
{
    private readonly EmailTemplateService _emailTemplateService = new EmailTemplateService();

    public async Task<UserDto> LoginAsync(LoginDto login)
    {
        var user = await userRepository.FindByEmailAsync(login.Email);

        if (!user.UserConfirmed)
        {
            throw new UnauthorizedException("Email is not verified.");
        }
        
        return new UserDto
        {
            Uuid = user.Uuid,
            Email = user.Email,
            DateCreated = user.DateCreated,
            DateUpdated = user.DateUpdated,
        };
    }

    public async Task RegisterAsync(RegisterDto registerData)
    {
        var user = await userRepository.AddUserAsync(registerData);
        await quizRepository.CreateUserQuizTableResultsAsync(user.Uuid);
        var token = await userRepository.AddVerifyEmailTokenAsync(registerData.Email);

        var htmlBody =
            _emailTemplateService.VerifyEmailTemplateHtml(
                $"{envVars.Origin}/verify?token={token}&email={registerData.Email}");
        
        await emailService.SendEmailAsync(registerData.Email, "Verify your email", htmlBody);
    }
    
    public async Task VerifyEmailAsync(VerifyEmailDto verifyData)
    {
        var user = await userRepository.FindByEmailAsync(verifyData.Email);

        if (user.VerifyEmailToken != verifyData.Token)
        {
            throw new UnauthorizedException("Token is not valid.");
        }

        await userRepository.VerifyUserByToken(verifyData.Email);
    }
}