using FlashcardsApp.Models;

namespace FlashcardsApp.Entities
{
    /// <summary>
    /// Entity depicting deck data. Deck has to be initially made by user, but if user gets deleted, 
    /// and deck was public then it stays in database with empty CreatorId.
    /// </summary>
    public class Deck
    {
        /// <summary>
        /// Deck's primary key.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id of user that created the deck.
        /// </summary>
        public int? CreatorId { get; set; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public User? Creator { get; set; }
        /// <summary>
        /// Title text of deck.
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// Description text of deck.
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Checks whether the deck is private or not.
        /// </summary>
        public bool IsPrivate { get; set; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public IEnumerable<UserDeck>? UserDecks { get; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public IEnumerable<Card>? Cards { get; set; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public IEnumerable<RevisionLog>? RevisionLogs { get; set; }
    }
}
