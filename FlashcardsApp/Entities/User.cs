using FlashcardsApp.Entities;

namespace FlashcardsApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; 
        public IEnumerable<Statistic>? Statistics { get; set; }
        public IEnumerable<UserDeck>? UserDecks { get; }
        public IEnumerable<Deck>? OwnerDecks { get; set; }
        public IEnumerable<RevisionLog>? RevisionLogs { get; set; }
    }
}
