using FlashcardsApp.Models;

namespace FlashcardsApp.Entities
{
    public class Deck
    {
        public int Id { get; set; }
        public int? CreatorId { get; set; }
        public User? Creator { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPrivate { get; set; }
        public IEnumerable<UserDeck>? UserDecks { get; }
        public IEnumerable<Card>? Cards { get; set; }
        public IEnumerable<RevisionLog>? RevisionLogs { get; set; }
    }
}
