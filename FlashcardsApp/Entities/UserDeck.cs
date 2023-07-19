using FlashcardsApp.Models;

namespace FlashcardsApp.Entities
{
    /// <summary>
    /// Weak entity used in depicting user's possession of decks.
    /// </summary>
    public class UserDeck
    {
        /// <summary>
        /// Concerned user.
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Deck possessed by user userId.
        /// </summary>
        public int DeckId { get; set; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public Deck Deck { get; set; }
    }
}
