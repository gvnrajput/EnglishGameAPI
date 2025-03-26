namespace EnglishGameAPI.Models
{
    public class Profile
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
