using FlashcardsApp.Models;

namespace FlashcardsApp.Entities
{
    public class UserDeck
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int DeckId { get; set; }
        public Deck Deck { get; set; }
    }
}
