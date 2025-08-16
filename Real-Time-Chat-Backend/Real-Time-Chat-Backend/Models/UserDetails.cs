namespace Real_Time_Chat_Backend.Models
{
    public class UserDetails
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int Age { get; set; }
        public User User { get; set; }

    }
}
