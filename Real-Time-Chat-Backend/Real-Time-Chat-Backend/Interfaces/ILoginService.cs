using Real_Time_Chat_Backend.Dtos;

namespace Real_Time_Chat_Backend.Interfaces
{
    public interface ILoginService
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<LoginResponse> Register(LoginRequest request);
        Task<List<UserDto>> GetAllUsers();
        Task<UserDto> GetUserById(int userId);
        Task<UserDto> InsertUser(UserCreationDto userDetails);
        Task<string> UpdateRoleById(int userId, string role);
        Task<LoginResponse> GoogleLogin(GoogleLoginRequest request);

    }
}