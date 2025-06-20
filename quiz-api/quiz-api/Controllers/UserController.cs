using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using quiz_api.Service;

namespace quiz_api.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<UserDataDto>> LoadUser([FromBody] LoadUserDto loadUserData)
        {
            if (!ModelState.IsValid)
            {
                throw new BadRequestException("Validation failed");
            }
            
            var user = await userService.LoadUser(loadUserData.Email);
            
            return Ok(user);
        }
    }
}