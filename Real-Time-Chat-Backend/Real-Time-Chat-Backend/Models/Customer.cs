namespace Real_Time_Chat_Backend.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string CustomerType { get; set; } = string.Empty;// could be an enum
        public string Status { get; set; } = string.Empty;// could be an enum
        public string AvatarColor { get; set; } = string.Empty;
    }
}
