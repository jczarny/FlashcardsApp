using FlashcardsApp.Models;

namespace FlashcardsApp.Entities
{
    public class RevisionLog
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int DeckId { get; set; }
        public Deck Deck { get; set; }
        public int CardId { get; set; }
        public Card Card { get; set; }
        public DateTime Date { get; set; }
        public int Ease { get; set; }
        public int Stage { get; set; }
    }
}
