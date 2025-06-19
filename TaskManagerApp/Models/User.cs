namespace TaskManagerApp.Models
{
    public class User
    {
        public int Id { get; set; } // optional, auto-increment
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
