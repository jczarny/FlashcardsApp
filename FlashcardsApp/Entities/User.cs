using FlashcardsApp.Entities;

namespace FlashcardsApp.Entities
{
    /// <summary>
    /// Entity depicting whole user's information and credentials.
    /// </summary>
    public class User
    {
        /// <summary>
        /// User's primary key.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// User's username.
        /// </summary>
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// Hashed password (using SHA256).
        /// </summary>
        public byte[] PasswordHash { get; set; } = new byte[256];
        /// <summary>
        /// Added salt for security.
        /// </summary>
        public byte[] PasswordSalt { get; set; } = new byte[256];

        /// <summary>
        /// Refresh token stored when user is currently logged in.
        /// </summary>
        public string? RefreshToken { get; set; }
        /// <summary>
        /// Refresh token date of creation.
        /// </summary>
        public DateTime? TokenCreated { get; set; }
        /// <summary>
        /// Refresh token expiration date.
        /// </summary>
        public DateTime? TokenExpires { get; set; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public IEnumerable<Statistic>? Statistics { get; set; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public IEnumerable<UserDeck>? UserDecks { get; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public IEnumerable<Deck>? OwnerDecks { get; set; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public IEnumerable<RevisionLog>? RevisionLogs { get; set; }
    }
}
