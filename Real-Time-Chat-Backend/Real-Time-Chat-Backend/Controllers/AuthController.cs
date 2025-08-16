using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Real_Time_Chat_Backend.Dtos;
using Real_Time_Chat_Backend.Interfaces;


namespace Real_Time_Chat_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public AuthController(
            ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            LoginResponse loginResponse = await _loginService.Login(request);
            return Ok(loginResponse);

        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> Register([FromBody] LoginRequest request)
        {
            LoginResponse loginResponse = await _loginService.Register(request);
            return Ok(loginResponse);
        }

        [HttpGet("getAllUsers")]
        public async Task<List<UserDto>> getAllUsers()
        {
            List<UserDto> Users = await _loginService.GetAllUsers();
            return Users;
        }

        [HttpGet("getUserById")]
        public async Task<UserDto> getUserById(int UserId)
        {
            UserDto User = await _loginService.GetUserById(UserId);
            return User;
        }

        [HttpPost("InsertNewUser")]
        public async Task<UserDto> InsertNewUser(UserCreationDto UserDetails)
        {
            UserDto User = await _loginService.InsertUser(UserDetails);
            return User;
        }

        [HttpPut("UpdateRole")]
        public async Task<string> UpdateRoleById(int userId, string role)
        {
            string messgae = await _loginService.UpdateRoleById(userId, role);
            return messgae;
        }

        [HttpPost("google")]
        public async Task<ActionResult<LoginResponse>> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            LoginResponse loginResponse = await _loginService.GoogleLogin(request);
            return Ok(loginResponse);
        }

    }
}
