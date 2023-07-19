using FlashcardsApp.Models;

namespace FlashcardsApp.Entities
{
    /// <summary>
    /// Entity which is the basis of learning process. \
    /// All cards encountered by a certain user will be stored here exactly once. \
    /// When user encouters certain card again, the log will be updated. \
    /// Otherwise it would lead to huge amount of records.
    /// </summary>
    public class RevisionLog
    {
        /// <summary>
        /// Id of learning user, part of primary key.
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Id of learned deck, part of primary key.
        /// </summary>
        public int DeckId { get; set; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public Deck Deck { get; set; }
        /// <summary>
        /// Id of learned card, part of primary key.
        /// </summary>
        public int CardId { get; set; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public Card Card { get; set; }
        /// <summary>
        /// Calculated date of next revision of card cardId by user userId
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Logged response of user, depicting user's proficiency on this card (Try again/Hard/Medium/Easy)
        /// </summary>
        public int Ease { get; set; }
        /// <summary>
        /// Logged history of previous responses, depicting whole history of previous encounters with this card.
        /// </summary>
        public int Stage { get; set; }
    }
}
