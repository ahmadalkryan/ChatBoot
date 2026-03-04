using Application.Dtos.UserDto;
using Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ArabicChatBot.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
      //  private readonly ILogger<AuthenticationController> _logger;
        private readonly IUserService _userService;

        public AuthenticationController(IUserService user )
        {
            _userService = user;

        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var res = await _userService.GetAllUser();
            var response = new
            {
                Result = true ,
                Message = "Success",
                Data = res


            };
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] Register register)
        {
            var result = await _userService.RegisterAsync(register);
            return Ok(new
            {
                Result = true,
                Message = "Success",
                Data = result
            });
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            var result = await _userService.LoginAsync(login);

            return Ok(new
            {
                Result =true ,
                Message="Success",
                Data = result
            });
        }
    }
}
