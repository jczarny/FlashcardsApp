namespace FlashcardsApp.Models
{

    /// <summary>
    /// Object used for easier Refresh Token management.
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// Actual token made for authenticating user.
        /// </summary>
        public string Token { get; set; } = string.Empty;
        /// <summary>
        /// Expiry date of token.
        /// </summary>
        public DateTime Expires { get; set; }
    }
}
