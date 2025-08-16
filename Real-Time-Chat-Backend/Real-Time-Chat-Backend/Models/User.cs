namespace Real_Time_Chat_Backend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Role { get; set; } = string.Empty;
        public virtual ICollection<UserDetails>? UserDetails { get; set; }
    }
}
