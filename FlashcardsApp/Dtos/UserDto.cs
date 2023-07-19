namespace FlashcardsApp.Dtos
{
    /// <summary>
    /// Simple Data transfer object used in login and registration forms.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// User's username.
        /// </summary>
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// User's password (not yet encrypted).
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
