namespace FlashcardsApp.Models
{
    /*
     * Token used for easier RefreshToken management
     */
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
    }
}
