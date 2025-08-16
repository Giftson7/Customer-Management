using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Real_Time_Chat_Backend.Data;
using Real_Time_Chat_Backend.Dtos;
using Real_Time_Chat_Backend.Interfaces;
using Real_Time_Chat_Backend.Models;

namespace Real_Time_Chat_Backend.Services
{
    public class LoginService : ILoginService
    {
        #region Declaration
        private readonly ApplicationDbContext _context;
        private readonly CommonService _commonService;
        private readonly IConfiguration _config;
        #endregion

        #region Constructor
        public LoginService(ApplicationDbContext context, CommonService commonService, IConfiguration config)
        {
            _context = context;
            _commonService = commonService;
            _config = config;
        }
        #endregion

        #region Login API
        public async Task<LoginResponse> Login(LoginRequest request)
        {
            try
            {
                // Check if user exists
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);

                if (user == null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password",
                        StatusCode = 204

                    };
                }

                // Generate simple token (in production, use JWT)
                var token = _commonService.GenerateJwtToken(user.Email, user.Role);

                return new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    StatusCode = 200,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email
                    }
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Internal server error",
                    StatusCode = 500
                };
            }
        }
        #endregion

        #region Register API
        public async Task<LoginResponse> Register(LoginRequest request)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username);

                if (existingUser != null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Message = "Username already exists",
                        StatusCode = 200
                    };
                }

                // Create new user
                var newUser = new User
                {
                    Username = request.Username,
                    Password = request.Password, // In production, hash the password
                    Email = $"{request.Username}@example.com",
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return new LoginResponse
                {
                    Success = true,
                    Message = "Registration successful",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Internal server error",
                    StatusCode = 500
                };
            }
        }
        #endregion

        #region GetAllUsers
        public async Task<List<UserDto>> GetAllUsers()
        {
            List<UserDto> users = new();

            List<User> userEntity = await _context.Users.ToListAsync();

            foreach (User user in userEntity)
            {
                UserDto currentUser = new();
                currentUser.Id = user.Id;
                currentUser.Email = user.Email;
                currentUser.Username = user.Username;
                currentUser.Password = user.Password;

                users.Add(currentUser);
            }

            return users;
        }
        #endregion

        #region GetUserById
        public async Task<UserDto> GetUserById(int userId)
        {
            UserDto user = new();

            if (userId < 0) return user;

            User? actualUser = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);

            if (actualUser == null) return user;

            user.Id = userId;
            user.Email = actualUser.Email;
            user.Username = actualUser.Username;


            return user;
        }
        #endregion

        #region InsertNewUser
        public async Task<UserDto> InsertUser(UserCreationDto userDetails)
        {

            if (userDetails == null) return new UserDto();

            //map dto to entity
            User newUser = new();
            newUser.Email = userDetails.Email;
            newUser.Password = userDetails.Password;
            newUser.Username = userDetails.Username;
            newUser.CreatedAt = DateTime.Now;

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            UserDto createdUser = new UserDto
            {
                Id = newUser.Id,
                Email = newUser.Email,
                Password = newUser.Password,
                Username = newUser.Username
            };

            return createdUser;
        }
        #endregion

        #region UpdateRoleById
        public async Task<string> UpdateRoleById(int userId, string role)
        {
            if (userId > 0 && string.IsNullOrEmpty(role)) return "failed to update";

            User? currentUser = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);

            if (currentUser == null) return "No user Found";

            currentUser.Role = role;

            await _context.SaveChangesAsync();

            return "Updated Successfully";

        }
        #endregion

        #region Login Using Google
        public async Task<LoginResponse> GoogleLogin(GoogleLoginRequest request)
        {
            try
            {
                // Validate Google token
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { _config["GoogleAuth:ClientId"] }
                    });

                // Check if the user exists in DB
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);
                if (user == null)
                {
                    // Create new user from Google data
                    user = new User
                    {
                        Email = payload.Email,
                        Username = payload.Name,
                        Password = Guid.NewGuid().ToString(), // random
                        Role = "User",
                        CreatedAt = DateTime.Now
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                // Generate JWT
                var token = _commonService.GenerateJwtToken(user.Email, user.Role);

                return new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    StatusCode = 200,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Email = user.Email
                    }
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid Google token",
                    StatusCode = 401
                };
            }
        }

        #endregion

    }


}
