using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Real_Time_Chat_Backend.Data;
using Real_Time_Chat_Backend.Dtos;
using Real_Time_Chat_Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Real_Time_Chat_Backend.Services
{
    [Route("api/Common")]
    [ApiController]
    //[Authorize]
    public class CommonService
    {
        #region Declaration 
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public CommonService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region GetAllUsers
        [HttpGet("GetUsers")]
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

        #region GetUsersBasedonRole
        [HttpGet("GetRoleBasedUsers")]
        public async Task<List<UserDto>> GetRoleBasedUsers(string role)
        {
            List<UserDto> users = new();

            List<User> userEntity = await _context.Users.Where(user => user.Role == role).ToListAsync();

            foreach (User user in userEntity)
            {
                UserDto currentUser = new();
                currentUser.Id = user.Id;
                currentUser.Email = user.Email;
                currentUser.Username = user.Username;
                currentUser.Password = user.Password;
                currentUser.Role = user.Role;

                users.Add(currentUser);
            }

            return users;
        }
        #endregion

        #region RemoveEntryById
        [HttpDelete("RemoveEntryById")]
        public async Task<String> RemoveEntryById(int id)
        {

            User? userEntity = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

            if (userEntity == null) return "No record found!!!";

            _context.Users.Remove(userEntity);

            await _context.SaveChangesAsync();

            return "Successfully removed";
        }
        #endregion

        #region GenerateJwtToken
        [HttpGet("GenerateToken")]
        [AllowAnonymous]
        public string GenerateJwtToken(string email, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim("SessionId" , Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this-is-my-jwt-token-im-using-it-for-learning-purpose-and-for-authentication")); // Use strong key!
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "Giftson",
                audience: "Samuel",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        #region GetSessionId
        [HttpGet("GetSessionId")]
        public Guid GetSessionId()
        {
            var sessionId = _httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "SessionId")?.Value;
            return string.IsNullOrEmpty(sessionId) ? Guid.Empty : Guid.Parse(sessionId);
        }
        #endregion

        #region InsertUserDetails
        [HttpPost("InsertUserDetails")]

        public async Task<UserDetailsDto> InsertUserDetails(UserDetailsDto userDetails)
        {
            if (userDetails == null) return null;

            UserDetails detailsEntity = new UserDetails
            {
                UserId = userDetails.UserId,
                PhoneNumber = userDetails.PhoneNumber,
                Address = userDetails.Address,
                Age = userDetails.Age,
            };

            await _context.UserDetails.AddAsync(detailsEntity);

            await _context.SaveChangesAsync();

            userDetails.Id = detailsEntity.Id;

            return userDetails;
        }
        #endregion

        #region GetAllUserDetails
        [HttpGet("GetAllUserDetails")]
        public async Task<List<UserDetailsDto>> GetAllUserDetails()
        {
            List<UserDetailsDto> userDetails = new();

            userDetails = await (from user in _context.Users
                                 join details in _context.UserDetails on user.Id equals details.UserId
                                 select new UserDetailsDto
                                 {
                                     Id = user.Id,
                                     Username = user.Username,
                                     Email = user.Email,
                                     Password = user.Password,
                                     UserId = details.UserId,
                                     PhoneNumber = details.PhoneNumber ?? "",
                                     Address = details.Address,
                                     Age = details.Age,
                                     Role = user.Role
                                 }).ToListAsync();

            return userDetails;
        }
        #endregion

        #region GetAllUserDetailsByUserId
        [HttpGet("GetAllUserDetailsByUserId")]
        public async Task<UserDetailsDto> GetAllUserDetailsByUserId(int UserId)
        {
            UserDetailsDto userDetails = new();

            userDetails = await (from user in _context.Users
                                 join details in _context.UserDetails on user.Id equals details.UserId
                                 where details.UserId == UserId
                                 select new UserDetailsDto
                                 {
                                     Id = user.Id,
                                     Username = user.Username,
                                     Email = user.Email,
                                     Password = user.Password,
                                     UserId = details.UserId,
                                     PhoneNumber = details.PhoneNumber ?? "",
                                     Address = details.Address,
                                     Age = details.Age,
                                     Role = user.Role
                                 }).FirstOrDefaultAsync() ?? new UserDetailsDto();

            return userDetails;
        }
        #endregion

        #region GetCustomers
        [HttpGet("GetCustomers")]

        public async Task<List<CustomerDto>> GetCustomers()
        {
            List<CustomerDto> customers = new();

            customers = await (from customer in _context.Customer
                               select new CustomerDto
                               {
                                   Id = customer.Id,
                                   FirstName = customer.FirstName,
                                   LastName = customer.LastName,
                                   Title = customer.Title,
                                   Email = customer.Email,
                                   Phone = customer.Phone,
                                   CustomerType = customer.CustomerType,
                                   Status = customer.Status,
                                   AvatarColor = customer.AvatarColor
                               }).ToListAsync();

            return customers;
        }
        #endregion

        #region AddtCustomers
        [HttpPost("AddCustomers")]

        public async Task<CustomerDto> AddCustomers(CustomerDto customerDetail)
        {
            Customer customer = new Customer
            {
                FirstName = customerDetail.FirstName,
                LastName = customerDetail.LastName,
                Title = customerDetail.Title,
                Email = customerDetail.Email,
                Phone = customerDetail.Phone,
                CustomerType = customerDetail.CustomerType,
                Status = customerDetail.Status,
                AvatarColor = customerDetail.AvatarColor
            };

            await _context.Customer.AddAsync(customer);
            await _context.SaveChangesAsync();

            customerDetail.Id = customer.Id;
            return customerDetail;
        }
        #endregion

    }
}
