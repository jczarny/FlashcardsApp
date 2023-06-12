using FlashcardsApp.Entities;

namespace FlashcardsApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[256];
        public byte[] PasswordSalt { get; set; } = new byte[256];

        public string? RefreshToken { get; set; }
        public DateTime? TokenCreated { get; set; }
        public DateTime? TokenExpires { get; set; }

        public IEnumerable<Statistic>? Statistics { get; set; }
        public IEnumerable<UserDeck>? UserDecks { get; }
        public IEnumerable<Deck>? OwnerDecks { get; set; }
        public IEnumerable<RevisionLog>? RevisionLogs { get; set; }
    }
}
