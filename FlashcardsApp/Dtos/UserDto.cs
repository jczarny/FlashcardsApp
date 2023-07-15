namespace FlashcardsApp.Dtos
{
    public class UserDto
    {
        public string Username { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string Password { get; set; } = string.Empty;
        public string? AccessToken { get; set; } = string.Empty;
    }
}
