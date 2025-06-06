using Microsoft.AspNetCore.Mvc;

namespace quiz_api.Controllers
{
    [Route("auth/user/login")]
    [ApiController]
    public class Login : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public Login(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost]
        public async Task<ActionResult<UserDto>> UserLogin([FromBody] LoginDto login)
        {
            var user = await _authService.LoginAsync(login);

            return Ok(user);
        }
    }
    
    [Route("auth/user/register")]
    [ApiController]
    public class Register : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public Register(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost]
        public async Task<ActionResult<UserDto>> UserRegister([FromBody] RegisterDto registerData)
        {
            if (!ModelState.IsValid)
            {
                throw new BadRequestException("Validation failed");
            }
            
            await _authService.RegisterAsync(registerData);

            var response = new ResponseSuccess("User is registered. Please verify your email.");
            return StatusCode(201, response);
        }
    }
    
    [Route("auth/user/email/verify")]
    [ApiController]
    public class EmailVerify : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public EmailVerify(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost]
        public async Task<ActionResult<UserDto>> UserEmailVerify([FromBody] VerifyEmailDto verifyData)
        {
            if (!ModelState.IsValid)
            {
                throw new BadRequestException("Validation failed");
            }
            
            await _authService.VerifyEmailAsync(verifyData);

            var response = new ResponseSuccess("Email is verified");
            return StatusCode(200, response);
        }
    }
}