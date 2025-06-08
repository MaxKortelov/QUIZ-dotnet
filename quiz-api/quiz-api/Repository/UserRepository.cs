using Microsoft.EntityFrameworkCore;
using quiz_api.Models;


public interface IUserRepository
{
    Task<User> FindByEmailAsync(string email);
    Task<User> AddUserAsync(RegisterDto registerData);
    Task<string> AddVerifyEmailTokenAsync(string email);
    Task VerifyUserByToken(string email);

}
public class UserRepository : IUserRepository
{
    private readonly QuizDbContext _context;
    private readonly CryptoService _cryptoService;

    public UserRepository(QuizDbContext context, CryptoService cryptoService)
    {
        _context = context;
        _cryptoService = cryptoService;
    }
    
    public async Task<User> FindByEmailAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (user != null)
        {
            return user;
        }
        
        throw new NotFoundException("User not found.");
    }
    
    public async Task<User> AddUserAsync(RegisterDto registerData)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == registerData.Email.ToLower());

        if (existingUser != null)
            throw new BadRequestException("User already exists");

        var (hash, salt) = _cryptoService.EncryptPassword(registerData.Password);

        var newUser = new User
        {
            Username = registerData.Username,
            Email = registerData.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            Deleted = false,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return newUser;
    }
    
    public async Task<string> AddVerifyEmailTokenAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (user == null)
            throw new NotFoundException("User not found.");

        var token = _cryptoService.UniqueId();
        user.VerifyEmailToken = token;

        await _context.SaveChangesAsync();

        return token;
    }

    public async Task VerifyUserByToken(string email)
    {
        var user = await FindByEmailAsync(email);
        
        user.VerifyEmailToken = null;
        user.UserConfirmed = true;
        
        await _context.SaveChangesAsync();
    }
}