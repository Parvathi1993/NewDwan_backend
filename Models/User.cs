namespace LoginAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        // public string Username { get; set; } = string.Empty;
        // public string Password { get; set; } = string.Empty;
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    public class GetDataAdmin
    {
        public string Data1 { get; set; }
        public string Data2 { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}